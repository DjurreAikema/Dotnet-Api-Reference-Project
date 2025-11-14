using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Core.Features.ChecklistItems.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommandHandler(IChecklistRepository repository) : IRequestHandler<UpdateChecklistItemCommand, ChecklistItemDto?>
{
    public async Task<ChecklistItemDto?> Handle(UpdateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify item's checklist belongs to authenticated user

        var existingItem = await repository.GetChecklistItemByIdAsync(request.Id);
        if (existingItem == null)
        {
            return null;
        }

        var item = new ChecklistItem
        {
            Id = request.Id,
            ChecklistId = existingItem.ChecklistId,
            Title = request.Title,
            Checked = existingItem.Checked
        };

        var updated = await repository.UpdateChecklistItemAsync(item);

        return updated == null
            ? null
            : new ChecklistItemDto(updated.Id, updated.ChecklistId, updated.Title, updated.Checked);
    }
}