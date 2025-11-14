using MediatR;
using QuickLists.Core.DTOs;

namespace QuickLists.Core.Features.Checklists.Queries.GetAllChecklists;

public record GetAllChecklistsQuery : IRequest<IEnumerable<ChecklistDto>>;