using QuickLists.Core.Models;

namespace QuickLists.Core.Interfaces;

public interface IChecklistItemRepository
{
    Task<IEnumerable<ChecklistItem>> GetChecklistItemsAsync(string checklistId);
    Task<ChecklistItem?> GetChecklistItemByIdAsync(string id);

    Task<ChecklistItem> CreateChecklistItemAsync(ChecklistItem item);
    Task<ChecklistItem?> UpdateChecklistItemAsync(ChecklistItem item);
    Task<bool> DeleteChecklistItemAsync(string id);
    Task<bool> ToggleChecklistItemAsync(string id);
    Task<bool> ResetChecklistItemsAsync(string checklistId);
}