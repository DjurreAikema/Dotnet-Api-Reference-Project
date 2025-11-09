namespace QuickLists.Core.Models;

public class Checklist
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // TODO: Add when implementing authentication
    // public string UserId {get;set;} = string.Empty;
    // public User User {get;set;} = null!;

    // --- Navigation properties
    public ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
}