using MediatR;
using QuickLists.Core.DTOs;
using QuickLists.Core.Features.ChecklistItems.Commands.CreateChecklistItem;
using QuickLists.Core.Features.ChecklistItems.Commands.DeleteChecklistItem;
using QuickLists.Core.Features.ChecklistItems.Commands.ResetChecklistItems;
using QuickLists.Core.Features.ChecklistItems.Commands.ToggleChecklistItem;
using QuickLists.Core.Features.ChecklistItems.Commands.UpdateChecklistItem;
using QuickLists.Core.Features.ChecklistItems.Queries.GetChecklistItems;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Api.Endpoints;

public static class ChecklistItemEndpoints
{
    public static void MapChecklistItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("ChecklistItems");

        // GET /api/checklists/{checklistId/items - Get items for checklist
        group.MapGet("/checklists/{checklistId}/items", async (
                string checklistId,
                IMediator mediator
            ) =>
            {
                var query = new GetChecklistItemsQuery(checklistId);
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("GetChecklistItems");

        // POST /api/checklists/{checklistId}/items - Create item
        group.MapPost("/checklists/{checklistId}/items", async (
                string checklistId,
                CreateChecklistItemDto dto,
                IMediator mediator
            ) =>
            {
                var command = new CreateChecklistItemCommand(checklistId, dto.Title);
                var result = await mediator.Send(command);
                return Results.Created($"/api/items/{result.Id}", result);
            })
            .WithName("CreateChecklistItem");

        // PUT /api/items/{id} - Update item
        group.MapPut("/items/{id}", async (
                string id,
                UpdateChecklistItemDto dto,
                IMediator mediator
            ) =>
            {
                var command = new UpdateChecklistItemCommand(id, dto.Title);
                var result = await mediator.Send(command);

                return result == null
                    ? Results.NotFound()
                    : Results.Ok(result);
            })
            .WithName("UpdateChecklistItem");

        // PATCH /api/items/{id}/toggle - Toggle checked status
        group.MapPatch("/items/{id}/toggle", async (
                string id,
                IMediator mediator
            ) =>
            {
                var command = new ToggleChecklistItemCommand(id);
                var result = await mediator.Send(command);

                return result == null
                    ? Results.NotFound()
                    : Results.Ok(result);
            })
            .WithName("ToggleChecklistItem");

        // PATCH /api/checklists/{checklistId}/reset - Reset all items
        group.MapPatch("/checklists/{checklistId}/reset", async (
                string checklistId,
                IMediator mediator
            ) =>
            {
                var command = new ResetChecklistItemsCommand(checklistId);
                var result = await mediator.Send(command);

                return result
                    ? Results.NoContent()
                    : Results.NotFound();
            })
            .WithName("ResetChecklistItems");

        // DELETE /api/items/{id} - Delete item
        group.MapDelete("/items/{id}", async (
                string id,
                IMediator mediator
            ) =>
            {
                var command = new DeleteChecklistItemCommand(id);
                var deleted = await mediator.Send(command);

                return deleted
                    ? Results.NoContent()
                    : Results.NotFound();
            })
            .WithName("DeleteChecklistItem");
    }
}