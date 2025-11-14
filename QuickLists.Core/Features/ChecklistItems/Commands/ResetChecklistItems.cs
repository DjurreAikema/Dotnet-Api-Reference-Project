using MediatR;
using QuickLists.Core.Caching;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.ChecklistItems.Commands;

// --- Command
public record ResetChecklistItemsCommand(string ChecklistId) : IRequest<bool>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeysToInvalidate =>
    [
        $"checklists:{ChecklistId}:items"
    ];
}

// --- Handler
public class ResetChecklistItemsCommandHandler(IChecklistRepository repository) : IRequestHandler<ResetChecklistItemsCommand, bool>
{
    public async Task<bool> Handle(ResetChecklistItemsCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify checklist belongs to authenticated user
        return await repository.ResetChecklistItemsAsync(request.ChecklistId);
    }
}