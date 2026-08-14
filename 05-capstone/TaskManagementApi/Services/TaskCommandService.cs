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
}
