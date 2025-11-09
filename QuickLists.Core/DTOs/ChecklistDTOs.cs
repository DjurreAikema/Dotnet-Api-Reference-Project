namespace QuickLists.Core.DTOs;

public record ChecklistDto(
    string Id,
    string Title
);

public record CreateChecklistDto(
    string Title
);

public record UpdateChecklistDto(
    string Title
);