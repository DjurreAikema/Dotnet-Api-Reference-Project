using MediatR;

namespace QuickLists.Core.Features.ChecklistItems.Commands.DeleteChecklistItem;

public record DeleteChecklistItemCommand(string Id) : IRequest<bool>;