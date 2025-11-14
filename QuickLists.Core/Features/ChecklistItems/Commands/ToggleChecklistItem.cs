using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.ChecklistItems.Commands;

// --- Command
public record ToggleChecklistItemCommand(string Id) : IRequest<ChecklistItemDto?>;

// --- Handler
public class ToggleChecklistItemCommandHandler(IChecklistRepository repository) : IRequestHandler<ToggleChecklistItemCommand, ChecklistItemDto?>
{
    public async Task<ChecklistItemDto?> Handle(ToggleChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify item's checklist belongs to authenticated user

        var toggled = await repository.ToggleChecklistItemAsync(request.Id);
        if (!toggled)
        {
            return null;
        }

        var item = await repository.GetChecklistItemByIdAsync(request.Id);

        return item == null
            ? null
            : new ChecklistItemDto(item.Id, item.ChecklistId, item.Title, item.Checked);
    }
}