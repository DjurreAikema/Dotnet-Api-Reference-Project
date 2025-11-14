using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.Checklists.Queries;

// --- Query
public record GetChecklistByIdQuery(string Id) : IRequest<ChecklistDto?>;

// --- Handler
public class GetChecklistByIdQueryHandler(IChecklistRepository repository) : IRequestHandler<GetChecklistByIdQuery, ChecklistDto?>
{
    public async Task<ChecklistDto?> Handle(GetChecklistByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO Add authorization check
        var checklist = await repository.GetChecklistByIdAsync(request.Id);

        return checklist == null
            ? null
            : new ChecklistDto(checklist.Id, checklist.Title);
    }
}