using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;

SQLitePCL.Batteries_V2.Init();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TaskDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'TaskDatabase' is not configured.");

builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { Status = "ready" }));

app.MapGet("/tasks", async (
    bool? completed,
    TaskDbContext dbContext,
    CancellationToken cancellationToken) =>
{
    IQueryable<TaskItem> query = dbContext.Tasks.AsNoTracking();

    if (completed is not null)
    {
        query = query.Where(task => task.IsCompleted == completed.Value);
    }

    var tasks = await query
        .OrderBy(task => task.Id)
        .Select(task => new TaskSummary(
            task.Id,
            task.Title,
            task.IsCompleted,
            task.Category == null ? null : task.Category.Name))
        .ToListAsync(cancellationToken);

    return Results.Ok(tasks);
});

app.Run();
