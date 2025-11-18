using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuickLists.Core.Caching;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.ChecklistItems.Commands;

// --- Command
public record ToggleChecklistItemCommand(string Id) : IRequest<ChecklistItemDto?>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeysToInvalidate => [];
}

// --- Handler
public class ToggleChecklistItemCommandHandler(IChecklistItemRepository repository, IMemoryCache cache) : IRequestHandler<ToggleChecklistItemCommand, ChecklistItemDto?>
{
    public async Task<ChecklistItemDto?> Handle(ToggleChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify item's checklist belongs to authenticated user
        var item = await repository.GetChecklistItemByIdAsync(request.Id);
        if (item == null)
        {
            return null;
        }

        var toggled = await repository.ToggleChecklistItemAsync(request.Id);
        if (!toggled)
        {
            return null;
        }

        cache.Remove($"checklists:{item.ChecklistId}:items");

        var updated = await repository.GetChecklistItemByIdAsync(request.Id);
        return updated == null
            ? null
            : new ChecklistItemDto(updated.Id, updated.ChecklistId, updated.Title, updated.Checked);
    }
}