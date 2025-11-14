using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuickLists.Core.Caching;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Core.Features.ChecklistItems.Commands;

// --- Command
public record UpdateChecklistItemCommand(string Id, string Title) : IRequest<ChecklistItemDto?>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeysToInvalidate => [];
}

// --- Validator
public class UpdateChecklistItemCommandValidator : AbstractValidator<UpdateChecklistItemCommand>
{
    public UpdateChecklistItemCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Item ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Length(1, 200)
            .WithMessage("Title must be between 1 and 200 characters");
    }
}

// --- Handler
public class UpdateChecklistItemCommandHandler(IChecklistRepository repository, IMemoryCache cache) : IRequestHandler<UpdateChecklistItemCommand, ChecklistItemDto?>
{
    public async Task<ChecklistItemDto?> Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify item's checklist belongs to authenticated user

        var existingItem = await repository.GetChecklistItemByIdAsync(request.Id);
        if (existingItem == null)
        {
            return null;
        }

        var item = new ChecklistItem
        {
            Id = request.Id,
            ChecklistId = existingItem.ChecklistId,
            Title = request.Title,
            Checked = existingItem.Checked
        };

        var updated = await repository.UpdateChecklistItemAsync(item);

        // Manually invalidate the items cache since we didn't have ChecklistId in the command
        cache.Remove($"checklists:{existingItem.ChecklistId}:items");

        return updated == null
            ? null
            : new ChecklistItemDto(updated.Id, updated.ChecklistId, updated.Title, updated.Checked);
    }
}