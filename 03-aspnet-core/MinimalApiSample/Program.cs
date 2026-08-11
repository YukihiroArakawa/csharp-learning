using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.Use(async (context, next) =>
{
    var startedAt = Stopwatch.GetTimestamp();
    app.Logger.LogInformation(
        "Request timing middleware entering for {Method} {Path}",
        context.Request.Method,
        context.Request.Path);

    await next(context);

    var elapsed = Stopwatch.GetElapsedTime(startedAt);
    app.Logger.LogInformation(
        "Request timing middleware leaving with {StatusCode} after {ElapsedMilliseconds} ms",
        context.Response.StatusCode,
        elapsed.TotalMilliseconds);
});

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("Response header middleware entering");
    context.Response.Headers["X-Task-Api"] = "MinimalApiSample";

    await next(context);

    app.Logger.LogInformation("Response header middleware leaving");
});

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
    var errors = Validate(request);
    IResult result;

    if (errors.Count > 0)
    {
        result = Results.ValidationProblem(errors);
    }
    else
    {
        var task = new TaskItem(tasks.Count + 1, request.Title, false);
        tasks.Add(task);
        result = Results.Created($"/tasks/{task.Id}", task);
    }

    return result;
});

app.Run();

static Dictionary<string, string[]> Validate(CreateTaskRequest request)
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(request.Title))
    {
        errors["title"] = new[] { "Title is required." };
    }
    else if (request.Title.Length > 100)
    {
        errors["title"] = new[] { "Title must be 100 characters or fewer." };
    }

    return errors;
}

record TaskItem(int Id, string Title, bool IsCompleted);

record CreateTaskRequest(string Title);
