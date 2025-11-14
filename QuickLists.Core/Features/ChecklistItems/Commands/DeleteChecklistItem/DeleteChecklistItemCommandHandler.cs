using MediatR;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.ChecklistItems.Commands.DeleteChecklistItem;

public class DeleteChecklistItemCommandHandler(IChecklistRepository repository) : IRequestHandler<DeleteChecklistItemCommand, bool>
{
    public async Task<bool> Handle(DeleteChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify item's checklist belongs to authenticated user
        return await repository.DeleteChecklistItemAsync(request.Id);
    }
}