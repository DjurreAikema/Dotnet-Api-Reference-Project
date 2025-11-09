namespace QuickLists.Core.DTOs;

public record ChecklistItemDto(
    string Id,
    string ChecklistId,
    string Title,
    bool Checked
);

public record CreateChecklistItemDto(
    string Title
);

public record UpdateChecklistItemDto(
    string Title
);