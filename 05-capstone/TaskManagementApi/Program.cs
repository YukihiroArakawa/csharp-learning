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

var app = builder.Build();

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
