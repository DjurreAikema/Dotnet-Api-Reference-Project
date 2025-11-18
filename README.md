# QuickLists API - .NET Architecture Reference

A production-ready .NET 9 Web API demonstrating modern backend architectural patterns. This project serves as a reference implementation of Clean Architecture, CQRS, and industry best practices for building maintainable, scalable APIs.

## Core Architectural Principles

This API is built around several fundamental architectural patterns:

### 1. Clean Architecture

The solution follows Clean Architecture principles with clear separation of concerns across three layers:

**Core Layer** ([`QuickLists.Core/`](QuickLists.Core/))
- Domain models and business logic
- DTOs and interfaces
- CQRS commands/queries
- Validation rules
- No external dependencies

**Infrastructure Layer** ([`QuickLists.Infrastructure/`](QuickLists.Infrastructure/))
- Data access with Entity Framework Core
- Repository implementations
- Database migrations
- External service integrations

**API Layer** ([`QuickLists.Api/`](QuickLists.Api/))
- HTTP endpoints (minimal APIs)
- Middleware and filters
- Dependency injection configuration
- Entry point and configuration

Dependencies flow inward: API → Infrastructure → Core. The Core layer has no dependencies on outer layers.

### 2. CQRS with MediatR

Commands and Queries are separated using the CQRS pattern, implemented with [MediatR](https://github.com/jbogard/MediatR):

**Commands** ([example: `CreateChecklist`](QuickLists.Core/Features/Checklists/Commands/CreateChecklist.cs))
- Modify state
- Return created/updated entities
- Trigger cache invalidation

**Queries** ([example: `GetAllChecklists`](QuickLists.Core/Features/Checklists/Queries/GetAllChecklists.cs))
- Read-only operations
- Can be cached
- Never modify state

Each feature is organized into its own folder with commands and queries co-located, making the codebase highly navigable and maintainable.

### 3. Pipeline Behaviors

MediatR pipeline behaviors provide cross-cutting concerns without cluttering business logic:

**Validation Pipeline** ([`ValidationBehavior.cs`](QuickLists.Core/Behaviors/ValidationBehavior.cs))
- Automatically validates all commands/queries using FluentValidation
- Throws `ValidationException` before handler execution if validation fails
- No validation code needed in handlers

**Caching Pipeline** ([`CachingBehavior.cs`](QuickLists.Core/Behaviors/CachingBehavior.cs))
- Automatically caches queries implementing `ICacheableQuery`
- Automatically invalidates cache for commands implementing `ICacheInvalidator`
- Pattern-based invalidation (e.g., `"checklists:*"` invalidates all checklist caches)

Behaviors execute in order: Validation → Caching → Handler.

## Key Patterns and Features

### Intelligent Caching

The caching system uses pattern-based invalidation with a registry:

- **Cacheable Queries**: Implement `ICacheableQuery` interface ([example](QuickLists.Core/Features/Checklists/Queries/GetAllChecklists.cs#L9-L12))
- **Cache Invalidation**: Commands specify which cache keys to invalidate ([example](QuickLists.Core/Features/Checklists/Commands/CreateChecklist.cs#L11-L17))
- **Pattern Matching**: Wildcard patterns like `checklists:*` invalidate all related caches
- **Metrics**: Built-in cache hit/miss tracking ([`CacheMetrics.cs`](QuickLists.Core/Caching/CacheMetrics.cs))

### FluentValidation Pipeline

All DTOs are validated automatically using FluentValidation:

```csharp
public class CreateChecklistCommandValidator : AbstractValidator<CreateChecklistCommand>
{
    public CreateChecklistCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(1, 200);
    }
}
```

Validators are registered automatically and execute before any handler runs. See [validation example](QuickLists.Core/Features/Checklists/Commands/CreateChecklist.cs#L19-L30).

### Problem Details Error Handling

Standardized error responses using RFC 7807 Problem Details:

- **Validation errors**: Return 400 with structured validation errors ([middleware](QuickLists.Api/Middleware/GlobalExceptionHandlerMiddleware.cs#L33-L59))
- **Unhandled exceptions**: Return 500 with problem details
- **Consistent format**: All errors follow the same structure

Example validation error response:
```json
{
  "type": "https://tools.ietf.org/html/rfc7807#section-6.5.1",
  "title": "One or more validation errors occurred",
  "status": 400,
  "errors": {
    "Title": ["Title is required"]
  }
}
```

### Integration Testing

Tests use xUnit with in-memory database for fast, isolated testing:

- **WebApplicationFactory**: Spins up the entire API in-memory ([`QuickListsApiFactory.cs`](QuickLists.Api.Tests/QuickListsApiFactory.cs))
- **In-memory database**: Each test gets a fresh database
- **Full workflow testing**: Tests cover entire request/response cycle ([example tests](QuickLists.Api.Tests/ChecklistEndpointsTests.cs))

No unit tests for simple CRUD operations with no business logic—integration tests provide better value.

### Structured Logging with Serilog

- **Serilog integration**: Structured logging to console and file ([`Program.cs`](QuickLists.Api/Program.cs#L12-L21))
- **Request logging**: Automatic HTTP request/response logging
- **Rolling files**: Daily log rotation with structured output
- **Context enrichment**: Correlation IDs and contextual information

## Project Structure

```
QuickLists.sln
├── QuickLists.Core/                  # Domain layer (no dependencies)
│   ├── Models/                       # Domain entities
│   ├── DTOs/                         # Data transfer objects
│   ├── Interfaces/                   # Repository interfaces
│   ├── Features/                     # CQRS commands and queries
│   │   ├── Checklists/
│   │   │   ├── Commands/
│   │   │   └── Queries/
│   │   └── ChecklistItems/
│   ├── Behaviors/                    # MediatR pipeline behaviors
│   └── Caching/                      # Caching abstractions
├── QuickLists.Infrastructure/        # Data access layer
│   └── Data/
│       ├── ApplicationDbContext.cs
│       ├── Repositories/
│       └── Migrations/
├── QuickLists.Api/                   # Presentation layer
│   ├── Endpoints/                    # Minimal API endpoints
│   ├── Middleware/                   # Custom middleware
│   └── Program.cs                    # App configuration
└── QuickLists.Api.Tests/            # Integration tests
    ├── QuickListsApiFactory.cs       # Test fixture
    └── *EndpointsTests.cs            # Test files
```

## Getting Started

**Prerequisites**: .NET 9 SDK

```bash
# Restore dependencies
dotnet restore

# Run database migrations
dotnet ef database update --project QuickLists.Infrastructure --startup-project QuickLists.Api

# Run the API
dotnet run --project QuickLists.Api

# API available at http://localhost:5000
# Swagger UI at http://localhost:5000/swagger
```

**Run tests**:
```bash
dotnet test
```

## Configuration

Configure connection strings and settings in [`appsettings.json`](QuickLists.Api/appsettings.json) and [`appsettings.Development.json`](QuickLists.Api/appsettings.Development.json).

CORS is configured to allow requests from `http://localhost:4200` for the Angular frontend.
