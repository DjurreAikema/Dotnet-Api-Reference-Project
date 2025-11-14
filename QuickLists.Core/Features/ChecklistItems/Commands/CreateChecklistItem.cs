using FluentValidation;
using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Core.Features.ChecklistItems.Commands;

// --- Command
public record CreateChecklistItemCommand(string ChecklistId, string Title) : IRequest<ChecklistItemDto>;

// --- Validator
public class CreateChecklistItemCommandValidator : AbstractValidator<CreateChecklistItemCommand>
{
    public CreateChecklistItemCommandValidator()
    {
        RuleFor(x => x.ChecklistId)
            .NotEmpty()
            .WithMessage("Checklist ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Length(1, 200)
            .WithMessage("Title must be between 1 and 200 characters");
    }
}

// --- Handler
public class CreateChecklistItemCommandHandler(IChecklistRepository repository) : IRequestHandler<CreateChecklistItemCommand, ChecklistItemDto>
{
    public async Task<ChecklistItemDto> Handle(CreateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify checklist exists and belongs to authenticated user

        var item = new ChecklistItem
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            ChecklistId = request.ChecklistId,
            Title = request.Title
        };

        var created = await repository.CreateChecklistItemAsync(item);

        return new ChecklistItemDto(
            created.Id,
            created.ChecklistId,
            created.Title,
            created.Checked
        );
    }
}