using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IBoardService _boardService;

    public TasksController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    // GET /api/tasks?columnId=...
    [HttpGet]
    public ActionResult<IReadOnlyList<TaskItem>> GetByColumn([FromQuery] Guid columnId)
    {
        if (columnId == Guid.Empty)
        {
            return BadRequest("columnId query parameter is required.");
        }

        return Ok(_boardService.GetTasksForColumn(columnId));
    }

    // GET /api/tasks/{id}
    [HttpGet("{id:guid}")]
    public ActionResult<TaskItem> GetById(Guid id)
    {
        var task = _boardService.GetTask(id);
        return task is null ? NotFound() : Ok(task);
    }

    // POST /api/tasks
    [HttpPost]
    public ActionResult<TaskItem> Create([FromBody] CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Task name is required.");
        }

        var task = _boardService.AddTask(request);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    // PUT /api/tasks/{id}
    [HttpPut("{id:guid}")]
    public ActionResult<TaskItem> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Task name is required.");
        }

        var updated = _boardService.UpdateTask(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE /api/tasks/{id}
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        return _boardService.DeleteTask(id) ? NoContent() : NotFound();
    }

    // PATCH /api/tasks/{id}/move
    [HttpPatch("{id:guid}/move")]
    public ActionResult<TaskItem> Move(Guid id, [FromBody] MoveTaskRequest request)
    {
        var moved = _boardService.MoveTask(id, request.ColumnId);
        return moved is null ? NotFound() : Ok(moved);
    }

    // PATCH /api/tasks/{id}/favorite
    [HttpPatch("{id:guid}/favorite")]
    public ActionResult<TaskItem> ToggleFavorite(Guid id)
    {
        var task = _boardService.ToggleFavorite(id);
        return task is null ? NotFound() : Ok(task);
    }
}
