using MediatR;
using QuickLists.Core.DTOs;

namespace QuickLists.Core.Features.ChecklistItems.Queries.GetChecklistItems;

public record GetChecklistItemsQuery(string ChecklistId) : IRequest<IEnumerable<ChecklistItemDto>>;