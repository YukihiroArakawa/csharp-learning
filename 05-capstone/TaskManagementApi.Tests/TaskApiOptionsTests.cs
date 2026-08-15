using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TaskManagementApi.Models;

namespace TaskManagementApi.Tests;

public sealed class TaskApiOptionsTests(TaskManagementApiFactory factory)
    : IClassFixture<TaskManagementApiFactory>
{
    [Fact]
    public async Task GetTasks_WhenPageSizeIsOmitted_UsesConfiguredDefault()
    {
        await factory.ResetDatabaseAsync();
        using var configuredFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TaskApi:DefaultPageSize"] = "2",
                    ["TaskApi:MaxPageSize"] = "3",
                });
            });
        });
        using var client = configuredFactory.CreateClient();

        var page = await client.GetFromJsonAsync<PagedResponse<TaskSummary>>("/tasks");

        Assert.NotNull(page);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(2, page.Items.Count);

        var invalidResponse = await client.GetAsync("/tasks?pageSize=4");
        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);
    }

    [Fact]
    public void CreateClient_WhenDefaultExceedsMaximum_ThrowsOptionsValidationException()
    {
        using var configuredFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["TaskApi:DefaultPageSize"] = "4",
                    ["TaskApi:MaxPageSize"] = "3",
                });
            });
        });

        Assert.Throws<OptionsValidationException>(
            configuredFactory.CreateClient);
    }
}
