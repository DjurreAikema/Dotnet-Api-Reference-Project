using MediatR;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.Checklists.Commands.DeleteChecklist;

public class DeleteChecklistCommandHandler(IChecklistRepository repository) : IRequestHandler<DeleteChecklistCommand, bool>
{
    public async Task<bool> Handle(DeleteChecklistCommand request, CancellationToken cancellationToken)
    {
        // TODO Add authorization check
        return await repository.DeleteChecklistAsync(request.Id);
    }
}