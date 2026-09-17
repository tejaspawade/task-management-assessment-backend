using System.Collections.Concurrent;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services;

/// <summary>
/// Simple thread-safe in-memory store for the board. Swapping this for a
/// database-backed implementation later just means implementing
/// IBoardService again - controllers don't need to change.
/// </summary>
public class BoardService : IBoardService
{
    private readonly ConcurrentDictionary<Guid, BoardColumn> _columns = new();
    private readonly ConcurrentDictionary<Guid, TaskItem> _tasks = new();

    public BoardService()
    {
        SeedDefaultColumns();
    }

    /// <summary>
    /// Seeds the board with default columns ("To Do", "In Progress", "Done").
    /// </summary>
    private void SeedDefaultColumns()
    {
        var defaults = new[] { "To Do", "In Progress", "Done" };
        for (var i = 0; i < defaults.Length; i++)
        {
            var column = new BoardColumn { Name = defaults[i], Order = i };
            _columns[column.Id] = column;
        }
    }

    /// <summary>
    /// Retrieves all board columns, ordered by their display order.
    /// </summary>
    public IReadOnlyList<BoardColumn> GetColumns() =>
        _columns.Values.OrderBy(c => c.Order).ToList();

    /// <summary>
    /// Adds a new board column with the specified name.
    /// </summary>
    /// <param name="name">The name of the new column.</param>
    /// <returns>The newly created board column.</returns>
    public BoardColumn AddColumn(string name)
    {
        var nextOrder = _columns.IsEmpty ? 0 : _columns.Values.Max(c => c.Order) + 1;
        var column = new BoardColumn { Name = name, Order = nextOrder };
        _columns[column.Id] = column;
        return column;
    }

    /// <summary>
    /// Retrieves all tasks for a specific column, ordered by favorite status and name.
    /// </summary>
    /// <param name="columnId">The ID of the column.</param>
    /// <returns>A list of tasks in the specified column.</returns>
    public IReadOnlyList<TaskItem> GetTasksForColumn(Guid columnId)
    {
        return _tasks.Values
            .Where(t => t.ColumnId == columnId)
            .OrderByDescending(t => t.IsFavorite)
            .ThenBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Retrieves a specific task by its ID.
    /// </summary>
    /// <param name="taskId">The ID of the task.</param>
    /// <returns>The task if found; otherwise, null.</returns>
    public TaskItem? GetTask(Guid taskId) =>
        _tasks.TryGetValue(taskId, out var task) ? task : null;

    /// <summary>
    /// Adds a new task to the board.
    /// </summary>
    /// <param name="request">The request containing task details.</param>
    /// <returns>The newly created task.</returns>
    public TaskItem AddTask(CreateTaskRequest request)
    {
        var task = new TaskItem
        {
            Name = request.Name,
            Description = request.Description,
            Deadline = request.Deadline,
            ColumnId = request.ColumnId
        };
        _tasks[task.Id] = task;
        return task;
    }

    /// <summary>
    /// Updates an existing task with new details.
    /// </summary>
    /// <param name="taskId">The ID of the task to update.</param>
    /// <param name="request">The request containing updated task details.</param>
    /// <returns>The updated task if found; otherwise, null.</returns>
    public TaskItem? UpdateTask(Guid taskId, UpdateTaskRequest request)
    {
        if (!_tasks.TryGetValue(taskId, out var task))
        {
            return null;
        }

        task.Name = request.Name;
        task.Description = request.Description;
        task.Deadline = request.Deadline;
        task.ColumnId = request.ColumnId;
        task.IsFavorite = request.IsFavorite;
        task.ImageUrl = request.ImageUrl;
        return task;
    }

    /// <summary>
    /// Deletes a task by its ID.
    /// </summary>
    /// <param name="taskId">The ID of the task to delete.</param>
    /// <returns>True if the task was deleted; otherwise, false.</returns>
    public bool DeleteTask(Guid taskId) => _tasks.TryRemove(taskId, out _);

    /// <summary>
    /// Moves a task to a different column.
    /// </summary>
    /// <param name="taskId">The ID of the task to move.</param>
    /// <param name="newColumnId">The ID of the new column.</param>
    /// <returns>The updated task if found; otherwise, null.</returns>
    public TaskItem? MoveTask(Guid taskId, Guid newColumnId)
    {
        if (!_tasks.TryGetValue(taskId, out var task))
        {
            return null;
        }

        task.ColumnId = newColumnId;
        return task;
    }

    /// <summary>
    /// Toggles the favorite status of a task.
    /// </summary>
    /// <param name="taskId">The ID of the task to toggle.</param>
    /// <returns>The updated task if found; otherwise, null.</returns>
    public TaskItem? ToggleFavorite(Guid taskId)
    {
        if (!_tasks.TryGetValue(taskId, out var task))
        {
            return null;
        }

        task.IsFavorite = !task.IsFavorite;
        return task;
    }
}
