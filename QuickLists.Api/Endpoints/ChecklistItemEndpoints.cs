using QuickLists.Core.DTOs;
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
                IChecklistRepository repository
            ) =>
            {
                // TODO: Verify checklist belongs to authenticated user
                var items = await repository.GetChecklistItemsAsync(checklistId);
                return Results.Ok(items.Select(i => new ChecklistItemDto(
                    i.Id,
                    i.ChecklistId,
                    i.Title,
                    i.Checked
                )));
            })
            .WithName("GetChecklistItems");

        // POST /api/checklists/{checklistId}/items - Create item
        group.MapPost("/checklists/{checklistId}/items", async (
                string checklistId,
                CreateChecklistItemDto dto,
                IChecklistRepository repository
            ) =>
            {
                // TODO: Verify checklist belongs to authenticated user
                var checklist = await repository.GetChecklistByIdAsync(checklistId);
                if (checklist == null)
                {
                    return Results.NotFound("Checklist not found");
                }

                var item = new ChecklistItem
                {
                    Id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                    ChecklistId = checklistId,
                    Title = dto.Title
                };

                var created = await repository.CreateChecklistItemAsync(item);
                var response = new ChecklistItemDto(
                    created.Id,
                    created.ChecklistId,
                    created.Title,
                    created.Checked
                );

                return Results.Created($"/api/items/{created.Id}", response);
            })
            .WithName("CreateChecklistItem");

        // PUT /api/items/{id} - Update item
        group.MapPut("/items/{id}", async (
                string id,
                UpdateChecklistItemDto dto,
                IChecklistRepository repository
            ) =>
            {
                // TODO: Verify item's checklist belongs to authenticated user
                var existingItem = await repository.GetChecklistItemByIdAsync(id);
                if (existingItem == null)
                {
                    return Results.NotFound();
                }

                var item = new ChecklistItem
                {
                    Id = id,
                    ChecklistId = existingItem.ChecklistId,
                    Title = dto.Title,
                    Checked = existingItem.Checked
                };

                var updated = await repository.UpdateChecklistItemAsync(item);
                if (updated == null)
                {
                    return Results.NotFound();
                }

                var response = new ChecklistItemDto(
                    updated.Id,
                    updated.ChecklistId,
                    updated.Title,
                    updated.Checked
                );
                return Results.Ok(response);
            })
            .WithName("UpdateChecklistItem");

        // PATCH /api/items/{id}/toggle - Toggle checked status
        group.MapPatch("/items/{id}/toggle", async (
                string id,
                IChecklistRepository repository
            ) =>
            {
                // TODO: Verify item's checklist belongs to authenticated user
                var toggled = await repository.ToggleChecklistItemAsync(id);
                if (!toggled)
                {
                    return Results.NotFound();
                }

                var item = await repository.GetChecklistItemByIdAsync(id);
                var response = new ChecklistItemDto(
                    item!.Id,
                    item.ChecklistId,
                    item.Title,
                    item.Checked
                );
                return Results.Ok(response);
            })
            .WithName("ToggleChecklistItem");

        // PATCH /api/checklists/{checklistId}/reset - Reset all items
        group.MapPatch("/checklists/{checklistId}/reset", async (
                string checklistId,
                IChecklistRepository repository
            ) =>
            {
                // TODO: Verify checklist belongs to authenticated user
                var reset = await repository.ResetChecklistItemsAsync(checklistId);
                return !reset ? Results.NotFound() : Results.NoContent();
            })
            .WithName("ResetChecklistItems");

        // DELETE /api/items/{id} - Delete item
        group.MapDelete("/items/{id}", async (
                string id,
                IChecklistRepository repository
            ) =>
            {
                // TODO: Verify item's checklist belongs to authenticated user
                var deleted = await repository.DeleteChecklistItemAsync(id);
                return !deleted ? Results.NotFound() : Results.NoContent();
            })
            .WithName("DeleteChecklistItem");
    }
}