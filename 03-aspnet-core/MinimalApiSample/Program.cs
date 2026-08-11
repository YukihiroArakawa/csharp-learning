var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var tasks = new List<TaskItem>
{
    new(1, "Minimal APIのコードを読む", false),
    new(2, "GETとPOSTを確認する", true)
};

app.MapGet("/tasks", (bool? completed) =>
{
    IEnumerable<TaskItem> result = tasks;

    if (completed is not null)
    {
        result = tasks.Where(task => task.IsCompleted == completed.Value);
    }

    return Results.Ok(result);
});

app.MapGet("/tasks/{id:int}", (int id) =>
{
    var task = tasks.FirstOrDefault(task => task.Id == id);
    IResult result = task is null
        ? Results.NotFound()
        : Results.Ok(task);

    return result;
});

app.MapPost("/tasks", (CreateTaskRequest request) =>
{
    var task = new TaskItem(tasks.Count + 1, request.Title, false);
    tasks.Add(task);

    return Results.Created($"/tasks/{task.Id}", task);
});

app.Run();

record TaskItem(int Id, string Title, bool IsCompleted);

record CreateTaskRequest(string Title);
