namespace QuickLists.Core.Models;

public class ChecklistItem
{
    public string Id { get; set; } = string.Empty;
    public string ChecklistId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public bool Checked { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // --- Navigation properties
    public Checklist Checklist { get; set; } = null!;
}