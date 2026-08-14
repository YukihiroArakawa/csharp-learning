using System.Net;
using System.Net.Http.Json;
using TaskManagementApi.Models;

namespace TaskManagementApi.Tests;

public sealed class TaskDetailTests(TaskManagementApiFactory factory)
    : IClassFixture<TaskManagementApiFactory>
{
    [Fact]
    public async Task GetTask_WhenTaskExists_ReturnsTaskAsJson()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/tasks/3");

        response.EnsureSuccessStatusCode();
        var task = await response.Content.ReadFromJsonAsync<TaskSummary>();
        Assert.NotNull(task);
        Assert.Equal(3, task.Id);
        Assert.Equal("テストを書く", task.Title);
        Assert.False(task.IsCompleted);
    }

    [Fact]
    public async Task GetTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/tasks/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
