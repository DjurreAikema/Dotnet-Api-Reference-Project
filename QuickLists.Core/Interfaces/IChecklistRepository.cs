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
}