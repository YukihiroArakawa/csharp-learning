using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementApi.Data;
using TaskManagementApi.Services;

namespace TaskManagementApi.Tests;

public sealed class CancellationTokenTests(TaskManagementApiFactory factory)
    : IClassFixture<TaskManagementApiFactory>
{
    [Fact]
    public async Task GetTasks_WhenTokenIsAlreadyCanceled_ThrowsOperationCanceledException()
    {
        await factory.ResetDatabaseAsync();
        await using var scope = factory.Services.CreateAsyncScope();
        var service = scope.ServiceProvider.GetRequiredService<TaskQueryService>();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            service.GetTasksAsync(1, 20, cancellation.Token));
    }

    [Fact]
    public async Task CreateTask_WhenTokenIsAlreadyCanceled_DoesNotInsertTask()
    {
        await factory.ResetDatabaseAsync();

        await using (var commandScope = factory.Services.CreateAsyncScope())
        {
            var service = commandScope.ServiceProvider
                .GetRequiredService<TaskCommandService>();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
                service.CreateTaskAsync("保存されないtask", cancellation.Token));
        }

        await using var verificationScope = factory.Services.CreateAsyncScope();
        var dbContext = verificationScope.ServiceProvider
            .GetRequiredService<TaskDbContext>();
        Assert.Equal(5, await dbContext.Tasks.CountAsync());
    }
}
