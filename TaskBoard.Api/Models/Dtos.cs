namespace TaskBoard.Api.Models;

/// <summary>
/// Request to create a new task.
/// </summary>
public record CreateTaskRequest(
    string Name,
    string? Description,
    DateTime? Deadline,
    Guid ColumnId);

/// <summary>
/// Request to update an existing task.
/// </summary>
public record UpdateTaskRequest(
    string Name,
    string? Description,
    DateTime? Deadline,
    Guid ColumnId,
    bool IsFavorite,
    string? ImageUrl);

/// <summary>
/// Request to move a task to a different column.
/// </summary>
public record MoveTaskRequest(Guid ColumnId);

/// <summary>
/// Request to create a new board column.
/// </summary>
public record CreateColumnRequest(string Name);
