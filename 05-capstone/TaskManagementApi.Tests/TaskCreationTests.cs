using System.Net;
using System.Net.Http.Json;
using TaskManagementApi.Models;

namespace TaskManagementApi.Tests;

public sealed class TaskCreationTests(TaskManagementApiFactory factory)
    : IClassFixture<TaskManagementApiFactory>
{
    [Fact]
    public async Task CreateTask_WhenTitleIsValid_ReturnsCreatedTaskThatCanBeFetched()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskRequest("  新しいtask  "));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<TaskSummary>();
        Assert.NotNull(created);
        Assert.Equal("新しいtask", created.Title);
        Assert.False(created.IsCompleted);
        Assert.Equal($"/tasks/{created.Id}", response.Headers.Location?.OriginalString);

        var fetched = await client.GetFromJsonAsync<TaskSummary>(
            $"/tasks/{created.Id}");
        Assert.Equal(created, fetched);
    }

    [Fact]
    public async Task CreateTask_WhenTitleIsWhitespace_ReturnsBadRequest()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskRequest("   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WhenTitleExceedsMaximumLength_ReturnsBadRequest()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskRequest(new string('a', 101)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
