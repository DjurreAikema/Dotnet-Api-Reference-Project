using Microsoft.EntityFrameworkCore;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Infrastructure.Data.Repositories;

public class ChecklistRepository(ApplicationDbContext context) : IChecklistRepository
{
    // --- Checklist Operations
    public async Task<IEnumerable<Checklist>> GetAllChecklistsAsync()
    {
        // TODO: Filter by UserId when implementing authentication

        return await context.Checklists
            // .Where(c => c.UserId == currentUserId)
            .Include(c => c.Items)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }

    public async Task<Checklist?> GetChecklistByIdAsync(string id)
    {
        // TODO: Add UserId check when implementing authentication

        return await context.Checklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);
        //  .FirstOrDefaultAsync(c => c.Id == id && c.UserId == currentUserId);
    }

    public async Task<Checklist> CreateChecklistAsync(Checklist checklist)
    {
        checklist.CreatedAt = DateTime.UtcNow;
        checklist.UpdatedAt = DateTime.UtcNow;
        // TODO: Set UserId when implementing authentication
        // checklist.UserId = currentUserId;

        context.Checklists.Add(checklist);
        await context.SaveChangesAsync();
        return checklist;
    }

    public async Task<Checklist?> UpdateChecklistAsync(Checklist checklist)
    {
        // TODO: Add UserId check when implementing authentication
        var existingChecklist = await context.Checklists
            .FirstOrDefaultAsync(c => c.Id == checklist.Id);

        if (existingChecklist == null)
        {
            return null;
        }

        existingChecklist.Title = checklist.Title;
        existingChecklist.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existingChecklist;
    }

    public async Task<bool> DeleteChecklistAsync(string id)
    {
        // TODO: Add UserId check when implementing authentication
        var checklist = await context.Checklists.FindAsync(id);
        if (checklist == null)
        {
            return false;
        }

        context.Checklists.Remove(checklist);
        await context.SaveChangesAsync();
        return true;
    }
}