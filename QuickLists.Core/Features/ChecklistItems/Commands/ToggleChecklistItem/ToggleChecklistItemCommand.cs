using MediatR;
using QuickLists.Core.DTOs;

namespace QuickLists.Core.Features.ChecklistItems.Commands.ToggleChecklistItem;

public record ToggleChecklistItemCommand(string Id) : IRequest<ChecklistItemDto?>;