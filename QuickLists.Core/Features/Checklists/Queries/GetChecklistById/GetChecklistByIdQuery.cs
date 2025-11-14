using MediatR;
using QuickLists.Core.DTOs;

namespace QuickLists.Core.Features.Checklists.Queries.GetChecklistById;

public record GetChecklistByIdQuery(string Id) : IRequest<ChecklistDto?>;