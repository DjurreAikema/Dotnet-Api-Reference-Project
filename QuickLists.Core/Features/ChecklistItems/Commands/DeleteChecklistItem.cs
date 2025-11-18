using MediatR;
using Microsoft.Extensions.Caching.Memory;
using QuickLists.Core.Caching;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.ChecklistItems.Commands;

// --- Command
public record DeleteChecklistItemCommand(string Id) : IRequest<bool>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeysToInvalidate => [];
}

// --- Handler
public class DeleteChecklistItemCommandHandler(IChecklistItemRepository repository, IMemoryCache cache) : IRequestHandler<DeleteChecklistItemCommand, bool>
{
    public async Task<bool> Handle(DeleteChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // Get the item first to know which checklist it belongs to
        var item = await repository.GetChecklistItemByIdAsync(request.Id);
        if (item == null)
        {
            return false;
        }

        var deleted = await repository.DeleteChecklistItemAsync(request.Id);

        if (deleted)
        {
            cache.Remove($"checklists:{item.ChecklistId}:items");
        }

        return deleted;
    }
}