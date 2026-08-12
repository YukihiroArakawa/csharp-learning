using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Services;

public sealed class TaskQueryService(TaskDbContext dbContext)
{
    public async Task<IReadOnlyList<TaskSummary>> GetTasksAsync(
        bool? completed,
        CancellationToken cancellationToken)
    {
        IQueryable<TaskItem> query = dbContext.Tasks.AsNoTracking();

        if (completed is not null)
        {
            query = query.Where(task => task.IsCompleted == completed.Value);
        }

        return await query
            .OrderBy(task => task.Id)
            .Select(task => new TaskSummary(
                task.Id,
                task.Title,
                task.IsCompleted,
                task.Category == null ? null : task.Category.Name))
            .ToListAsync(cancellationToken);
    }
}
