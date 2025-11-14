using FluentValidation;

namespace QuickLists.Core.DTOs.Validators;

public class UpdateChecklistItemDtoValidator : AbstractValidator<UpdateChecklistItemDto>
{
    public UpdateChecklistItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Length(1, 200)
            .WithMessage("Title must be between 1 and 200 characters");
    }
}