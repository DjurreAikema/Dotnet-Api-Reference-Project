using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Core.Features.ChecklistItems.Commands.CreateChecklistItem;

public class CreateChecklistItemCommandHandler(IChecklistRepository repository) : IRequestHandler<CreateChecklistItemCommand, ChecklistItemDto>
{
    public async Task<ChecklistItemDto> Handle(CreateChecklistItemCommand request, CancellationToken cancellationToken)
    {
        // TODO Verify checklist exists and belongs to authenticated user

        var item = new ChecklistItem
        {
            Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            ChecklistId = request.ChecklistId,
            Title = request.Title
        };

        var created = await repository.CreateChecklistItemAsync(item);

        return new ChecklistItemDto(
            created.Id,
            created.ChecklistId,
            created.Title,
            created.Checked
        );
    }
}