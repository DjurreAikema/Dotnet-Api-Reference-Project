using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;

namespace QuickLists.Core.Features.Checklists.Queries.GetChecklistById;

public class GetChecklistByIdQueryHandler : IRequestHandler<GetChecklistByIdQuery, ChecklistDto?>
{
    private readonly IChecklistRepository _repository;

    public GetChecklistByIdQueryHandler(IChecklistRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChecklistDto?> Handle(GetChecklistByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO Add authorization check
        var checklist = await _repository.GetChecklistByIdAsync(request.Id);

        return checklist == null
            ? null
            : new ChecklistDto(checklist.Id, checklist.Title);
    }
}