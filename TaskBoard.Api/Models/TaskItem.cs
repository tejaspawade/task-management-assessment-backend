namespace TaskBoard.Api.Models;

/// <summary>
/// A single task/card on the board.
/// </summary>
public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Name of the task.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the task.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Optional deadline for the task.
    /// </summary>
    public DateTime? Deadline { get; set; }

    /// <summary>
    /// The Id of the Column this task currently lives in.
    /// </summary>
    public Guid ColumnId { get; set; }

    /// <summary>
    /// Indicates if the task is marked as a favorite.
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Optional data-URI or hosted URL for an attached image.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Timestamp when the task was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
