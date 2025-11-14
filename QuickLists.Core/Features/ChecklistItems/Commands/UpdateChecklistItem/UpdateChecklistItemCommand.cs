using MediatR;
using QuickLists.Core.DTOs;

namespace QuickLists.Core.Features.ChecklistItems.Commands.UpdateChecklistItem;

public record UpdateChecklistItemCommand(string Id, string Title) : IRequest<ChecklistItemDto?>;