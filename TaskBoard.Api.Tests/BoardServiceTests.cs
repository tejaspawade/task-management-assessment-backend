using NUnit.Framework;
using TaskBoard.Api.Models;
using TaskBoard.Api.Services;

namespace TaskBoard.Api.Tests;

[TestFixture]
public class BoardServiceTests
{
    private BoardService _service = null!;
    private Guid _columnId;

    [SetUp]
    public void SetUp()
    {
        _service = new BoardService();
        // BoardService seeds "To Do", "In Progress", "Done" on construction.
        _columnId = _service.GetColumns().First().Id;
    }

    [Test]
    public void Constructor_SeedsThreeDefaultColumns()
    {
        var columns = _service.GetColumns();

        Assert.That(columns, Has.Count.EqualTo(3));
        Assert.That(columns.Select(c => c.Name),
            Is.EqualTo(new[] { "To Do", "In Progress", "Done" }));
    }

    [Test]
    public void AddColumn_AppendsWithIncrementingOrder()
    {
        var newColumn = _service.AddColumn("Backlog");

        var columns = _service.GetColumns();
        Assert.That(columns.Last().Id, Is.EqualTo(newColumn.Id));
        Assert.That(newColumn.Order, Is.EqualTo(3));
    }

    [Test]
    public void AddTask_PersistsTaskWithGivenDetails()
    {
        var request = new CreateTaskRequest("Write tests", "Cover BoardService", DateTime.UtcNow.AddDays(1), _columnId);

        var created = _service.AddTask(request);

        var fetched = _service.GetTask(created.Id);
        Assert.That(fetched, Is.Not.Null);
        Assert.That(fetched!.Name, Is.EqualTo("Write tests"));
        Assert.That(fetched.Description, Is.EqualTo("Cover BoardService"));
        Assert.That(fetched.ColumnId, Is.EqualTo(_columnId));
    }

    [Test]
    public void UpdateTask_OverwritesEditableFields()
    {
        var created = _service.AddTask(new CreateTaskRequest("Draft", null, null, _columnId));
        var updateRequest = new UpdateTaskRequest(
            "Final name", "Final description", DateTime.UtcNow, _columnId, IsFavorite: true, ImageUrl: "http://img/1.png");

        var updated = _service.UpdateTask(created.Id, updateRequest);

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.Name, Is.EqualTo("Final name"));
        Assert.That(updated.Description, Is.EqualTo("Final description"));
        Assert.That(updated.IsFavorite, Is.True);
        Assert.That(updated.ImageUrl, Is.EqualTo("http://img/1.png"));
    }

    [Test]
    public void UpdateTask_ReturnsNull_WhenTaskDoesNotExist()
    {
        var result = _service.UpdateTask(Guid.NewGuid(),
            new UpdateTaskRequest("x", null, null, _columnId, false, null));

        Assert.That(result, Is.Null);
    }

    [Test]
    public void DeleteTask_RemovesTask_AndReturnsTrue()
    {
        var created = _service.AddTask(new CreateTaskRequest("Temp", null, null, _columnId));

        var deleted = _service.DeleteTask(created.Id);

        Assert.That(deleted, Is.True);
        Assert.That(_service.GetTask(created.Id), Is.Null);
    }

    [Test]
    public void DeleteTask_ReturnsFalse_WhenTaskDoesNotExist()
    {
        Assert.That(_service.DeleteTask(Guid.NewGuid()), Is.False);
    }

    [Test]
    public void MoveTask_ChangesColumnId()
    {
        var columns = _service.GetColumns();
        var targetColumn = columns[1].Id;
        var created = _service.AddTask(new CreateTaskRequest("Movable", null, null, _columnId));

        var moved = _service.MoveTask(created.Id, targetColumn);

        Assert.That(moved, Is.Not.Null);
        Assert.That(moved!.ColumnId, Is.EqualTo(targetColumn));
    }

    [Test]
    public void GetTasksForColumn_SortsAlphabeticallyByName()
    {
        _service.AddTask(new CreateTaskRequest("Charlie", null, null, _columnId));
        _service.AddTask(new CreateTaskRequest("alpha", null, null, _columnId));
        _service.AddTask(new CreateTaskRequest("Bravo", null, null, _columnId));

        var names = _service.GetTasksForColumn(_columnId).Select(t => t.Name).ToList();

        Assert.That(names, Is.EqualTo(new[] { "alpha", "Bravo", "Charlie" }));
    }

    [Test]
    public void GetTasksForColumn_PinsFavoritesToTop_ThenAlphabetical()
    {
        var zebra = _service.AddTask(new CreateTaskRequest("Zebra", null, null, _columnId));
        _service.AddTask(new CreateTaskRequest("Apple", null, null, _columnId));
        var mango = _service.AddTask(new CreateTaskRequest("Mango", null, null, _columnId));

        _service.ToggleFavorite(zebra.Id);
        _service.ToggleFavorite(mango.Id);

        var names = _service.GetTasksForColumn(_columnId).Select(t => t.Name).ToList();

        // Favorites (Mango, Zebra) alphabetically first, then the rest (Apple).
        Assert.That(names, Is.EqualTo(new[] { "Mango", "Zebra", "Apple" }));
    }

    [Test]
    public void ToggleFavorite_FlipsBooleanEachCall()
    {
        var created = _service.AddTask(new CreateTaskRequest("Task", null, null, _columnId));

        var firstToggle = _service.ToggleFavorite(created.Id);
        var secondToggle = _service.ToggleFavorite(created.Id);

        Assert.That(firstToggle!.IsFavorite, Is.True);
        Assert.That(secondToggle!.IsFavorite, Is.False);
    }

    [Test]
    public void GetTasksForColumn_OnlyReturnsTasksBelongingToThatColumn()
    {
        var otherColumnId = _service.GetColumns()[1].Id;
        _service.AddTask(new CreateTaskRequest("In first column", null, null, _columnId));
        _service.AddTask(new CreateTaskRequest("In second column", null, null, otherColumnId));

        var tasksInFirst = _service.GetTasksForColumn(_columnId);

        Assert.That(tasksInFirst, Has.Count.EqualTo(1));
        Assert.That(tasksInFirst.Single().Name, Is.EqualTo("In first column"));
    }
}
