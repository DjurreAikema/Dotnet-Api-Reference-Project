using MediatR;
using QuickLists.Core.Caching;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.ChecklistItems.Queries;

// --- Query
public record GetChecklistItemsQuery(string ChecklistId) : IRequest<IEnumerable<ChecklistItemDto>>, ICacheableQuery
{
    public string CacheKey => $"checklists:{ChecklistId}:items";
}

// --- Handler
public class GetChecklistItemsQueryHandler(IChecklistRepository repository) : IRequestHandler<GetChecklistItemsQuery, IEnumerable<ChecklistItemDto>>
{
    public async Task<IEnumerable<ChecklistItemDto>> Handle(GetChecklistItemsQuery request, CancellationToken cancellationToken)
    {
        // TODO Verify checklist belongs to authenticated user
        var items = await repository.GetChecklistItemsAsync(request.ChecklistId);

        return items.Select(i => new ChecklistItemDto(
            i.Id,
            i.ChecklistId,
            i.Title,
            i.Checked
        ));
    }
}