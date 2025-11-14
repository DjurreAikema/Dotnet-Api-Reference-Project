using MediatR;

namespace QuickLists.Core.Features.Checklists.Commands.DeleteChecklist;

public record DeleteChecklistCommand(string Id) : IRequest<bool>;