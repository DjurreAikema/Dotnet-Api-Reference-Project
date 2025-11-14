using MediatR;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.ChecklistItems.Commands.ResetChecklistItems;

public class ResetChecklistItemsCommandHandler(IChecklistRepository repository) : IRequestHandler<ResetChecklistItemsCommand, bool>
{
    public async Task<bool> Handle(ResetChecklistItemsCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify checklist belongs to authenticated user
        return await repository.ResetChecklistItemsAsync(request.ChecklistId);
    }
}