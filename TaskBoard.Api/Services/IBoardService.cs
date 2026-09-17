using TaskBoard.Api.Models;

namespace TaskBoard.Api.Services;

public interface IBoardService
{
    /// <summary>
    /// Retrieves all board columns.
    /// </summary>
    IReadOnlyList<BoardColumn> GetColumns();

    /// <summary>
    /// Adds a new board column with the specified name.
    /// </summary>
    /// <param name="name">The name of the new column.</param>
    /// <returns>The newly created board column.</returns>
    BoardColumn AddColumn(string name);

    /// <summary>
    /// Retrieves all tasks for a specific column, ordered by favorite status and name.
    /// </summary>
    /// <param name="columnId">The ID of the column.</param>
    /// <returns>A list of tasks in the specified column.</returns>
    IReadOnlyList<TaskItem> GetTasksForColumn(Guid columnId);

    /// <summary>
    /// Retrieves a specific task by its ID.
    /// </summary>
    /// <param name="taskId">The ID of the task.</param>
    /// <returns>The task if found; otherwise, null.</returns>
    TaskItem? GetTask(Guid taskId);

    /// <summary>
    /// Adds a new task to the board.
    /// </summary>
    /// <param name="request">The request containing task details.</param>
    /// <returns>The newly created task.</returns>
    TaskItem AddTask(CreateTaskRequest request);

    /// <summary>
    /// Updates an existing task with new details.
    /// </summary>
    /// <param name="taskId">The ID of the task to update.</param>
    /// <param name="request">The request containing updated task details.</param>
    /// <returns>The updated task if found; otherwise, null.</returns>
    TaskItem? UpdateTask(Guid taskId, UpdateTaskRequest request);

    /// <summary>
    /// Deletes a task by its ID.
    /// </summary>
    /// <param name="taskId">The ID of the task to delete.</param>
    /// <returns>True if the task was deleted; otherwise, false.</returns>
    bool DeleteTask(Guid taskId);

    /// <summary>
    /// Moves a task to a different column.
    /// </summary>
    /// <param name="taskId">The ID of the task to move.</param>
    /// <param name="newColumnId">The ID of the new column.</param>
    /// <returns>The updated task if found; otherwise, null.</returns>
    TaskItem? MoveTask(Guid taskId, Guid newColumnId);

    /// <summary>
    /// Toggles the favorite status of a task.
    /// </summary>
    /// <param name="taskId">The ID of the task to toggle.</param>
    /// <returns>The updated task if found; otherwise, null.</returns>
    TaskItem? ToggleFavorite(Guid taskId);
}
