using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskManagementApi.Models;

namespace TaskManagementApi.Tests;

public sealed class TaskPaginationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetTasks_WhenSecondPageIsRequested_ReturnsTwoItemsAndMetadata()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/tasks?page=2&pageSize=2");

        response.EnsureSuccessStatusCode();
        var page = await response.Content
            .ReadFromJsonAsync<PagedResponse<TaskSummary>>();

        Assert.NotNull(page);
        Assert.Equal(2, page.Page);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(5, page.TotalCount);
        Assert.Equal([3, 4], page.Items.Select(task => task.Id));
    }

    [Fact]
    public async Task GetTasks_WhenPageIsLessThanOne_ReturnsBadRequest()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/tasks?page=0&pageSize=2");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_WhenPageIsPastLastPage_ReturnsEmptyItems()
    {
        using var client = factory.CreateClient();

        var page = await client.GetFromJsonAsync<PagedResponse<TaskSummary>>(
            "/tasks?page=4&pageSize=2");

        Assert.NotNull(page);
        Assert.Empty(page.Items);
        Assert.Equal(5, page.TotalCount);
    }
}
