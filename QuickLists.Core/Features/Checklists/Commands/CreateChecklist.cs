using FluentValidation;
using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Core.Features.Checklists.Commands;

// --- Command
public record CreateChecklistCommand(string Title) : IRequest<ChecklistDto>;

// --- Validator
public class CreateChecklistCommandValidator : AbstractValidator<CreateChecklistCommand>
{
    public CreateChecklistCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Length(1, 200)
            .WithMessage("Title must be between 1 and 200 characters");
    }
}

// --- Handler
public class CreateChecklistCommandHandler(IChecklistRepository repository) : IRequestHandler<CreateChecklistCommand, ChecklistDto>
{
    public async Task<ChecklistDto> Handle(CreateChecklistCommand request, CancellationToken cancellationToken)
    {
        var checklist = new Checklist
        {
            Id = GenerateSlug(request.Title),
            Title = request.Title
            // TODO Set UserId when implementing authentication
        };

        var created = await repository.CreateChecklistAsync(checklist);
        return new ChecklistDto(created.Id, created.Title);
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "");

        return $"{slug}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    }
}