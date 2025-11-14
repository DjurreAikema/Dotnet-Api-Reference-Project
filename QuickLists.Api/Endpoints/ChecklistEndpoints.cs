using QuickLists.Core.DTOs;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Api.Endpoints;

public static class ChecklistEndpoints
{
    public static void MapChecklistEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checklists")
            .WithTags("Checklists");

        // GET /api/checklists - Get all checklists
        group.MapGet("/", async (IChecklistRepository repository) =>
            {
                // TODO: Get currentUserId from authenticated user context
                var checklists = await repository.GetAllChecklistsAsync();
                return Results.Ok(checklists.Select(c => new ChecklistDto(c.Id, c.Title)));
            })
            .WithName("GetAllChecklists");

        // GET /api/checklists/{id} - Get single checklist
        group.MapGet("/{id}", async (string id, IChecklistRepository repository) =>
            {
                // TODO: Verify checklist belongs to authenticated user
                var checklist = await repository.GetChecklistByIdAsync(id);
                if (checklist == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(new ChecklistDto(checklist.Id, checklist.Title));
            })
            .WithName("GetChecklistById");

        // POST /api/checklists - Create checklist
        group.MapPost("/", async (CreateChecklistDto dto, IChecklistRepository repository) =>
            {
                var checklist = new Checklist
                {
                    Id = GenerateSlug(dto.Title),
                    Title = dto.Title
                    // TODO: Set UserId from authenticated user
                    // UserId = currentUserId
                };

                var created = await repository.CreateChecklistAsync(checklist);
                var response = new ChecklistDto(created.Id, created.Title);

                return Results.Created($"/api/checklists/{created.Id}", response);
            })
            .WithName("CreateChecklist");

        // PUT /api/checklists/{id} - Update checklist
        group.MapPut("/{id}", async (string id, UpdateChecklistDto dto, IChecklistRepository repository) =>
            {
                // TODO: Verify checklist belongs to authenticated user
                var checklist = new Checklist
                {
                    Id = id,
                    Title = dto.Title
                };

                var updated = await repository.UpdateChecklistAsync(checklist);
                if (updated == null)
                {
                    return Results.NotFound();
                }

                var response = new ChecklistDto(updated.Id, updated.Title);
                return Results.Ok(response);
            })
            .WithName("UpdateChecklist");

        // DELETE /api/checklists/{id} - Delete checklist
        group.MapDelete("/{id}", async (string id, IChecklistRepository repository) =>
            {
                var deleted = await repository.DeleteChecklistAsync(id);
                return !deleted ? Results.NotFound() : Results.NoContent();
            })
            .WithName("DeleteChecklist");
    }

    // --- Methods
    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "");

        return $"{slug}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    }
}