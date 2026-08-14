using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("TaskDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'TaskDatabase' is not configured.");

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<TaskQueryService>();
builder.Services.AddScoped<TaskCommandService>();

var app = builder.Build();

app.MapPut("/tasks/{id:int}", async Task<IResult> (
    int id,
    UpdateTaskRequest request,
    TaskCommandService taskCommandService,
    CancellationToken cancellationToken) =>
{
    var title = request.Title?.Trim();
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(title))
    {
        errors[nameof(request.Title)] = ["titleは必須です。"];
    }
    else if (title.Length > 100)
    {
        errors[nameof(request.Title)] = ["titleは100文字以下で指定してください。"];
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var task = await taskCommandService.UpdateTaskAsync(
        id,
        title!,
        request.IsCompleted,
        cancellationToken);

    return task is null
        ? Results.NotFound()
        : Results.Ok(task);
});

app.MapPost("/tasks", async Task<IResult> (
    CreateTaskRequest request,
    TaskCommandService taskCommandService,
    CancellationToken cancellationToken) =>
{
    var title = request.Title?.Trim();
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(title))
    {
        errors[nameof(request.Title)] = ["titleは必須です。"];
    }
    else if (title.Length > 100)
    {
        errors[nameof(request.Title)] = ["titleは100文字以下で指定してください。"];
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var task = await taskCommandService.CreateTaskAsync(
        title!,
        cancellationToken);

    return Results.Created($"/tasks/{task.Id}", task);
});

app.MapGet("/tasks/{id:int}", async Task<IResult> (
    int id,
    TaskQueryService taskQueryService,
    CancellationToken cancellationToken) =>
{
    var task = await taskQueryService.GetTaskAsync(id, cancellationToken);

    return task is null
        ? Results.NotFound()
        : Results.Ok(task);
});

app.MapGet("/tasks", async Task<IResult> (
    TaskQueryService taskQueryService,
    CancellationToken cancellationToken,
    int page = 1,
    int pageSize = 20) =>
{
    var errors = new Dictionary<string, string[]>();

    if (page < 1)
    {
        errors[nameof(page)] = ["pageは1以上を指定してください。"];
    }

    if (pageSize is < 1 or > 100)
    {
        errors[nameof(pageSize)] = ["pageSizeは1以上100以下を指定してください。"];
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var result = await taskQueryService.GetTasksAsync(
        page,
        pageSize,
        cancellationToken);

    return Results.Ok(result);
});

app.Run();

public partial class Program;
