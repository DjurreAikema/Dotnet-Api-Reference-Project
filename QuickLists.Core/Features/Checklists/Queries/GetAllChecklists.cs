using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.Checklists.Queries;

// --- Query
public record GetAllChecklistsQuery : IRequest<IEnumerable<ChecklistDto>>;

// --- Handler
public class GetAllChecklistsQueryHandler(IChecklistRepository repository) : IRequestHandler<GetAllChecklistsQuery, IEnumerable<ChecklistDto>>
{
    public async Task<IEnumerable<ChecklistDto>> Handle(GetAllChecklistsQuery request, CancellationToken cancellationToken)
    {
        // TODO When implementing auth, filter by current user
        var checklists = await repository.GetAllChecklistsAsync();

        return checklists.Select(c => new ChecklistDto(c.Id, c.Title));
    }
}