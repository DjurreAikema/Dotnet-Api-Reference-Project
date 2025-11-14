using MediatR;

namespace QuickLists.Core.Features.ChecklistItems.Commands.ResetChecklistItems;

public record ResetChecklistItemsCommand(string ChecklistId) : IRequest<bool>;