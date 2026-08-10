var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var tasks = new List<TaskItem>
{
    new(1, "Minimal APIのコードを読む", false)
};

app.MapGet("/tasks", () => Results.Ok(tasks));

app.MapPost("/tasks", (CreateTaskRequest request) =>
{
    var task = new TaskItem(tasks.Count + 1, request.Title, false);
    tasks.Add(task);

    return Results.Created($"/tasks/{task.Id}", task);
});

app.Run();

record TaskItem(int Id, string Title, bool IsCompleted);

record CreateTaskRequest(string Title);
