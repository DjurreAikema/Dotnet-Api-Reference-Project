using MediatR;
using QuickLists.Core.DTOs;

namespace QuickLists.Core.Features.Checklists.Commands.CreateChecklist;

public record CreateChecklistCommand(string Title) : IRequest<ChecklistDto>;