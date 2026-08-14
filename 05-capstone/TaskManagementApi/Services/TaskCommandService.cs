using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services;

public sealed class TaskCommandService(TaskDbContext dbContext)
{
    public async Task<TaskSummary> CreateTaskAsync(
        string title,
        CancellationToken cancellationToken)
    {
        var task = new TaskItem
        {
            Title = title,
            IsCompleted = false,
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TaskSummary(
            task.Id,
            task.Title,
            task.IsCompleted);
    }

    public async Task<TaskSummary?> UpdateTaskAsync(
        int id,
        string title,
        bool isCompleted,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.SingleOrDefaultAsync(
            task => task.Id == id,
            cancellationToken);

        if (task is null)
        {
            return null;
        }

        task.Title = title;
        task.IsCompleted = isCompleted;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TaskSummary(
            task.Id,
            task.Title,
            task.IsCompleted);
    }

    public async Task<bool> DeleteTaskAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var task = await dbContext.Tasks.SingleOrDefaultAsync(
            task => task.Id == id,
            cancellationToken);

        if (task is null)
        {
            return false;
        }

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
