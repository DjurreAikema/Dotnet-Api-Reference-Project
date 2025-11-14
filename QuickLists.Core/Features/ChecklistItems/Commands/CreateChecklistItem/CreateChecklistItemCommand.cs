using MediatR;
using QuickLists.Core.DTOs;

namespace QuickLists.Core.Features.ChecklistItems.Commands.CreateChecklistItem;

public record CreateChecklistItemCommand(string ChecklistId, string Title) : IRequest<ChecklistItemDto>;