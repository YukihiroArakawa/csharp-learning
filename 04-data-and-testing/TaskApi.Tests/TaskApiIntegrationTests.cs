using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Tests;

public sealed class TaskApiIntegrationTests(TaskApiFactory factory)
    : IClassFixture<TaskApiFactory>
{
    [Fact]
    public async Task GetTasks_WhenCompletedIsFalse_ReturnsIncompleteTasksAsJson()
    {
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
            await dbContext.Database.EnsureCreatedAsync();

            dbContext.Tasks.AddRange(
                new TaskItem { Id = 2, Title = "完了済み", IsCompleted = true },
                new TaskItem { Id = 1, Title = "統合テストを読む", IsCompleted = false });
            await dbContext.SaveChangesAsync();
        }

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/tasks?completed=false");

        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskSummary>>();
        var task = Assert.Single(Assert.IsType<List<TaskSummary>>(tasks));
        Assert.Equal(1, task.Id);
        Assert.Equal("統合テストを読む", task.Title);
        Assert.False(task.IsCompleted);
    }
}
