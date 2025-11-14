using MediatR;
using QuickLists.Core.Caching;
using QuickLists.Core.Features.Checklists.Commands;
using QuickLists.Core.Features.Checklists.Queries;

namespace QuickLists.Api.Endpoints;

public static class ChecklistEndpoints
{
    public static void MapChecklistEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checklists")
            .WithTags("Checklists");

        // GET /api/checklists - Get all checklists
        group.MapGet("/", async (IMediator mediator) =>
            {
                var query = new GetAllChecklistsQuery();
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetAllChecklists");

        // GET /api/checklists/{id} - Get single checklist
        group.MapGet("/{id}", async (string id, IMediator mediator) =>
            {
                var query = new GetChecklistByIdQuery(id);
                var result = await mediator.Send(query);

                return result == null
                    ? Results.NotFound()
                    : Results.Ok(result);
            })
            .WithName("GetChecklistById");

        // POST /api/checklists - Create checklist
        group.MapPost("/", async (CreateChecklistCommand command, IMediator mediator) =>
            {
                var result = await mediator.Send(command);
                return Results.Created($"/api/checklists/{result.Id}", result);
            })
            .WithName("CreateChecklist");

        // PUT /api/checklists/{id} - Update checklist
        group.MapPut("/{id}", async (string id, UpdateChecklistCommand command, IMediator mediator) =>
            {
                var updateCommand = command with {Id = id};
                var result = await mediator.Send(updateCommand);

                return result == null
                    ? Results.NotFound()
                    : Results.Ok(result);
            })
            .WithName("UpdateChecklist");

        // DELETE /api/checklists/{id} - Delete checklist
        group.MapDelete("/{id}", async (string id, IMediator mediator) =>
            {
                var command = new DeleteChecklistCommand(id);
                var deleted = await mediator.Send(command);

                return deleted
                    ? Results.NoContent()
                    : Results.NotFound();
            })
            .WithName("DeleteChecklist");

        // TODO Move this endpoint
        group.MapGet("/cache/stats", (ICacheMetrics metrics) =>
            {
                var stats = metrics.GetStatistics();
                return Results.Ok(new
                {
                    stats.TotalHits,
                    stats.TotalMisses,
                    HitRate = $"{stats.HitRate:P2}"
                });
            })
            .WithName("GetCacheStatistics")
            .ExcludeFromDescription();
    }
}