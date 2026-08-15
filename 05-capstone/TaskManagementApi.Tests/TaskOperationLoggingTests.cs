using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskManagementApi.Models;
using TaskManagementApi.Services;

namespace TaskManagementApi.Tests;

public sealed class TaskOperationLoggingTests(TaskManagementApiFactory factory)
    : IClassFixture<TaskManagementApiFactory>
{
    [Fact]
    public async Task CreateTask_WhenSaveSucceeds_WritesStructuredTaskId()
    {
        await factory.ResetDatabaseAsync();
        var loggerProvider = new CollectingLoggerProvider();
        using var configuredFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
                services.AddSingleton<ILoggerProvider>(loggerProvider));
        });
        using var client = configuredFactory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskRequest("ログを確認する"));
        response.EnsureSuccessStatusCode();

        var log = Assert.Single(loggerProvider.Logs, log =>
            log.Category == typeof(TaskCommandService).FullName &&
            log.Template == "Created task {TaskId}");

        Assert.Equal(LogLevel.Information, log.Level);
        Assert.True(log.Properties.TryGetValue("TaskId", out var taskId));
        var numericTaskId = Assert.IsType<int>(taskId);
        Assert.True(numericTaskId > 0);
    }
}
