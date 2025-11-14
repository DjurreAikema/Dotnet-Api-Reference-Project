using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Core.Features.Checklists.Commands.UpdateChecklist;

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