using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Services;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TaskDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'TaskDatabase' is not configured.");

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<TaskQueryService>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { Status = "ready" }));

app.MapGet("/tasks", async (
    bool? completed,
    TaskQueryService taskQueryService,
    CancellationToken cancellationToken) =>
{
    var tasks = await taskQueryService.GetTasksAsync(completed, cancellationToken);

    return Results.Ok(tasks);
});

app.Run();

public partial class Program;
