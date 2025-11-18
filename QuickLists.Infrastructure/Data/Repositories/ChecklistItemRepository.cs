using Microsoft.EntityFrameworkCore;
using QuickLists.Core.Interfaces;
using QuickLists.Core.Models;

namespace QuickLists.Infrastructure.Data.Repositories;

public class ChecklistItemRepository(ApplicationDbContext context) : IChecklistItemRepository
{
    public async Task<IEnumerable<ChecklistItem>> GetChecklistItemsAsync(string checklistId)
    {
        // TODO: Verify checklist belongs to user when implementing authentication
        return await context.ChecklistItems
            .Where(i => i.ChecklistId == checklistId)
            .OrderBy(i => i.CreatedAt)
            .ToListAsync();
    }

    public async Task<ChecklistItem?> GetChecklistItemByIdAsync(string id)
    {
        // TODO: Verify item's checklist belongs to user when implementing authentication
        return await context.ChecklistItems.FindAsync(id);
    }

    public async Task<ChecklistItem> CreateChecklistItemAsync(ChecklistItem item)
    {
        // TODO: Verify checklist belongs to user when implementing authentication
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        item.Checked = false;

        context.ChecklistItems.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<ChecklistItem?> UpdateChecklistItemAsync(ChecklistItem item)
    {
        // TODO: Verify item's checklist belongs to user when implementing authentication
        var existingItem = await context.ChecklistItems.FindAsync(item.Id);
        if (existingItem == null)
        {
            return null;
        }

        existingItem.Title = item.Title;
        existingItem.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existingItem;
    }

    public async Task<bool> DeleteChecklistItemAsync(string id)
    {
        // TODO: Verify item's checklist belongs to user when implementing authentication
        var item = await context.ChecklistItems.FindAsync(id);
        if (item == null)
        {
            return false;
        }

        context.ChecklistItems.Remove(item);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleChecklistItemAsync(string id)
    {
        // TODO: Verify item's checklist belongs to user when implementing authentication
        var item = await context.ChecklistItems.FindAsync(id);
        if (item == null)
        {
            return false;
        }

        item.Checked = !item.Checked;
        item.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResetChecklistItemsAsync(string checklistId)
    {
        // TODO: Verify checklist belongs to user when implementing authentication
        var items = await context.ChecklistItems
            .Where(i => i.ChecklistId == checklistId)
            .ToListAsync();

        if (items.Count == 0)
        {
            return false;
        }

        foreach (var item in items)
        {
            item.Checked = false;
            item.UpdatedAt = DateTime.UtcNow;
        }

        await context.SaveChangesAsync();
        return true;
    }
}