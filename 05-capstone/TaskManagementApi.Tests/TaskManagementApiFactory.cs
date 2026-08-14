using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskManagementApi.Data;
using TaskManagementApi.Models;

namespace TaskManagementApi.Tests;

public sealed class TaskManagementApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection;

    public TaskManagementApiFactory()
    {
        SQLitePCL.Batteries_V2.Init();
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TaskDbContext>>();
            services.AddDbContext<TaskDbContext>(options =>
                options.UseSqlite(connection));
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        dbContext.Tasks.AddRange(
            new TaskItem { Id = 1, Title = "要件を確認する", IsCompleted = true },
            new TaskItem { Id = 2, Title = "ページングを実装する", IsCompleted = false },
            new TaskItem { Id = 3, Title = "テストを書く", IsCompleted = false },
            new TaskItem { Id = 4, Title = "READMEを更新する", IsCompleted = false },
            new TaskItem { Id = 5, Title = "変更をレビューする", IsCompleted = false });
        await dbContext.SaveChangesAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            connection.Dispose();
        }
    }
}
