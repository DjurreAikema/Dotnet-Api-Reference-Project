using MediatR;
using QuickLists.Core.Caching;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.Checklists.Commands;

// --- Command
public record DeleteChecklistCommand(string Id) : IRequest<bool>, ICacheInvalidator
{
    public IEnumerable<string> CacheKeysToInvalidate =>
    [
        $"checklists:{Id}", $"checklists:{Id}:items", "checklists:all"
    ];
}

// --- Handler
public class DeleteChecklistCommandHandler(IChecklistRepository repository) : IRequestHandler<DeleteChecklistCommand, bool>
{
    public async Task<bool> Handle(DeleteChecklistCommand request, CancellationToken cancellationToken)
    {
        // TODO Add authorization check
        return await repository.DeleteChecklistAsync(request.Id);
    }
}