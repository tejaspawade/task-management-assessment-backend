namespace TaskBoard.Api.Models;

/// <summary>
/// A work-state column on the board (e.g. To Do, In Progress, Done).
/// </summary>
public class BoardColumn
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Name of the column (e.g. To Do, In Progress, Done).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Left-to-right display order of the column on the board.
    /// </summary>
    public int Order { get; set; }
}
