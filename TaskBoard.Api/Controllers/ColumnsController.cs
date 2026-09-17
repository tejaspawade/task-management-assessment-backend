using Microsoft.AspNetCore.Mvc;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColumnsController : ControllerBase
{
    private readonly IBoardService _boardService;

    public ColumnsController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    // GET /api/columns
    [HttpGet]
    public ActionResult<IReadOnlyList<BoardColumn>> GetAll()
    {
        return Ok(_boardService.GetColumns());
    }

    // POST /api/columns
    [HttpPost]
    public ActionResult<BoardColumn> Create([FromBody] CreateColumnRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Column name is required.");
        }

        var column = _boardService.AddColumn(request.Name);
        return CreatedAtAction(nameof(GetAll), column);
    }
}
