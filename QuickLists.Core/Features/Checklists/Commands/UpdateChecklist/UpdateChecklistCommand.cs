using MediatR;
using QuickLists.Core.DTOs;

namespace QuickLists.Core.Features.Checklists.Commands.UpdateChecklist;

public record UpdateChecklistCommand(string Id, string Title) : IRequest<ChecklistDto?>;