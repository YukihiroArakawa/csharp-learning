using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TaskManagementApi.Data;
using TaskManagementApi.Models;
using TaskManagementApi.Options;
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
builder.Services
    .AddOptions<TaskApiOptions>()
    .Bind(builder.Configuration.GetSection(TaskApiOptions.SectionName))
    .Validate(options => options.DefaultPageSize >= 1,
        "TaskApi:DefaultPageSize must be at least 1.")
    .Validate(options => options.MaxPageSize is >= 1 and <= 500,
        "TaskApi:MaxPageSize must be between 1 and 500.")
    .Validate(options => options.DefaultPageSize <= options.MaxPageSize,
        "TaskApi:DefaultPageSize must not exceed TaskApi:MaxPageSize.")
    .ValidateOnStart();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;
    };
});

var app = builder.Build();

app.UseExceptionHandler();

app.MapDelete("/tasks/{id:int}", async Task<IResult> (
    int id,
    TaskCommandService taskCommandService,
    CancellationToken cancellationToken) =>
{
    var deleted = await taskCommandService.DeleteTaskAsync(
        id,
        cancellationToken);

    return deleted
        ? Results.NoContent()
        : Results.NotFound();
});

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
    IOptions<TaskApiOptions> options,
    CancellationToken cancellationToken,
    int page = 1,
    int? pageSize = null) =>
{
    var settings = options.Value;
    var effectivePageSize = pageSize ?? settings.DefaultPageSize;
    var errors = new Dictionary<string, string[]>();

    if (page < 1)
    {
        errors[nameof(page)] = ["pageは1以上を指定してください。"];
    }

    if (effectivePageSize < 1 || effectivePageSize > settings.MaxPageSize)
    {
        errors[nameof(pageSize)] =
            [$"pageSizeは1以上{settings.MaxPageSize}以下を指定してください。"];
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    var result = await taskQueryService.GetTasksAsync(
        page,
        effectivePageSize,
        cancellationToken);

    return Results.Ok(result);
});

app.Run();

public partial class Program;
