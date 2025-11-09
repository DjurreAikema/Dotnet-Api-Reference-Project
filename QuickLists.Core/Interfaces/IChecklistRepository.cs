using QuickLists.Core.DTOs;
using QuickLists.Core.Models;

namespace QuickLists.Core.Interfaces;

public interface IChecklistRepository
{
    // --- Checklist operations
    Task<IEnumerable<Checklist>> GetAllChecklistsAsync();
    Task<Checklist?> GetChecklistByIdAsync(string id);
    Task<Checklist> CreateChecklistAsync(Checklist checklist);
    Task<Checklist?> UpdateChecklistAsync(Checklist checklist);
    Task<bool> DeleteChecklistAsync(string id);

    // --- ChecklistItem operations
    Task<IEnumerable<ChecklistItem>> GetChecklistItemsAsync(string checklistId);
    Task<ChecklistItem?> GetChecklistItemByIdAsync(string id);
    Task<ChecklistItem> CreateChecklistItemAsync(ChecklistItem item);
    Task<ChecklistItem?> UpdateChecklistItemAsync(ChecklistItem item);
    Task<bool> DeleteChecklistItemAsync(string id);
    Task<bool> ToggleChecklistItemAsync(string id);
    Task<bool> ResetChecklistItemsAsync(string checklistId);
}