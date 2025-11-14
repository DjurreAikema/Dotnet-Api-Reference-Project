using FluentValidation;

namespace QuickLists.Core.Features.ChecklistItems.Commands.CreateChecklistItem;

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