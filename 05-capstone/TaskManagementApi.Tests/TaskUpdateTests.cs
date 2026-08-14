using System.Net;
using System.Net.Http.Json;
using TaskManagementApi.Models;

namespace TaskManagementApi.Tests;

public sealed class TaskUpdateTests(TaskManagementApiFactory factory)
    : IClassFixture<TaskManagementApiFactory>
{
    [Fact]
    public async Task UpdateTask_WhenInputIsValid_ReturnsUpdatedTaskThatCanBeFetched()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/tasks/2",
            new UpdateTaskRequest("  ページングを修正する  ", true));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<TaskSummary>();
        Assert.NotNull(updated);
        Assert.Equal(2, updated.Id);
        Assert.Equal("ページングを修正する", updated.Title);
        Assert.True(updated.IsCompleted);

        var fetched = await client.GetFromJsonAsync<TaskSummary>("/tasks/2");
        Assert.Equal(updated, fetched);
    }

    [Fact]
    public async Task UpdateTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/tasks/999",
            new UpdateTaskRequest("存在しないtask", true));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_WhenTitleIsWhitespace_ReturnsBadRequest()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/tasks/2",
            new UpdateTaskRequest("   ", true));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTask_WhenTitleExceedsMaximumLength_ReturnsBadRequest()
    {
        await factory.ResetDatabaseAsync();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/tasks/2",
            new UpdateTaskRequest(new string('a', 101), true));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
