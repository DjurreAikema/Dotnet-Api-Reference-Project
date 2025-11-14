using FluentValidation;
using MediatR;
using QuickLists.Core.Caching;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Core.Features.Checklists.Commands;

// --- Command
public record UpdateChecklistCommand(string Id, string Title) : IRequest<ChecklistDto?>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeysToInvalidate =>
    [
        $"checklists:{Id}", "checklists:all"
    ];
}

// --- Validator
public class UpdateChecklistCommandValidator : AbstractValidator<UpdateChecklistCommand>
{
    public UpdateChecklistCommandValidator()
    {
        RuleFor(x => x.Id)
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
public class UpdateChecklistCommandHandler(IChecklistRepository repository) : IRequestHandler<UpdateChecklistCommand, ChecklistDto?>
{
    public async Task<ChecklistDto?> Handle(UpdateChecklistCommand request, CancellationToken cancellationToken)
    {
        // TODO Add authorization check

        var checklist = new Checklist
        {
            Id = request.Id,
            Title = request.Title
        };

        var updated = await repository.UpdateChecklistAsync(checklist);
        return updated == null
            ? null
            : new ChecklistDto(updated.Id, updated.Title);
    }
}