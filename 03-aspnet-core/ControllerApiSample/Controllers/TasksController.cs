using Microsoft.AspNetCore.Mvc;
using ControllerApiSample.Models;

namespace ControllerApiSample.Controllers;

[ApiController]
[Route("controller/tasks")]
public sealed class TasksController : ControllerBase
{
    private static readonly List<TaskItem> Tasks =
    [
        new(1, "Controller APIのコードを読む", false),
        new(2, "Minimal APIと比較する", true)
    ];

    [HttpGet]
    [ProducesResponseType<IEnumerable<TaskItem>>(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TaskItem>> Get([FromQuery] bool? completed)
    {
        IEnumerable<TaskItem> result = Tasks;

        if (completed is not null)
        {
            result = Tasks.Where(task => task.IsCompleted == completed.Value);
        }

        return Ok(result);
    }

    [HttpGet("{id:int}", Name = nameof(GetById))]
    [ProducesResponseType<TaskItem>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskItem> GetById(int id)
    {
        var task = Tasks.FirstOrDefault(task => task.Id == id);

        return task is null
            ? NotFound()
            : Ok(task);
    }

    [HttpPost]
    [ProducesResponseType<TaskItem>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<TaskItem> Create([FromBody] CreateTaskRequest request)
    {
        var task = new TaskItem(Tasks.Count + 1, request.Title, false);
        Tasks.Add(task);

        return CreatedAtRoute(nameof(GetById), new { id = task.Id }, task);
    }
}
