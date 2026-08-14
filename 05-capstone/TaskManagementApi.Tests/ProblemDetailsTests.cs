using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagementApi.Data;

namespace TaskManagementApi.Tests;

public sealed class ProblemDetailsTests
{
    [Fact]
    public async Task GetTasks_WhenDatabaseFails_ReturnsProblemDetailsWithoutExceptionDetails()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<TaskDbContext>>();
                    services.AddDbContext<TaskDbContext>(options =>
                        options.UseSqlite("Data Source=:memory:"));
                });
            });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(500, problem.Status);
        Assert.False(string.IsNullOrWhiteSpace(problem.Title));
        Assert.False(string.IsNullOrWhiteSpace(problem.Type));
        Assert.True(problem.Extensions.ContainsKey("traceId"));

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("SqliteException", responseBody);
        Assert.DoesNotContain("StackTrace", responseBody);
    }
}
