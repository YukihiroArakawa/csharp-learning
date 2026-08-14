using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services;

public sealed class TaskQueryService(TaskDbContext dbContext)
{
    public Task<TaskSummary?> GetTaskAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return dbContext.Tasks
            .AsNoTracking()
            .Where(task => task.Id == id)
            .Select(task => new TaskSummary(
                task.Id,
                task.Title,
                task.IsCompleted))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResponse<TaskSummary>> GetTasksAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Tasks.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(task => task.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(task => new TaskSummary(
                task.Id,
                task.Title,
                task.IsCompleted))
            .ToListAsync(cancellationToken);

        return new PagedResponse<TaskSummary>(
            items,
            page,
            pageSize,
            totalCount);
    }
}
