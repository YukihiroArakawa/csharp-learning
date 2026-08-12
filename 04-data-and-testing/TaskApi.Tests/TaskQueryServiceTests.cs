using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;
using TaskApi.Services;

namespace TaskApi.Tests;

public sealed class TaskQueryServiceTests
{
    [Fact]
    public async Task GetTasksAsync_WhenCompletedIsFalse_ReturnsIncompleteTasksInIdOrder()
    {
        SQLitePCL.Batteries_V2.Init();

        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TaskDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new TaskDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();

        var learning = new TaskCategory { Name = "学習" };
        dbContext.Tasks.AddRange(
            new TaskItem { Id = 3, Title = "テストを読む", IsCompleted = false },
            new TaskItem { Id = 1, Title = "EF Coreを学ぶ", IsCompleted = false, Category = learning },
            new TaskItem { Id = 2, Title = "migrationを作る", IsCompleted = true });
        await dbContext.SaveChangesAsync();

        var service = new TaskQueryService(dbContext);

        var tasks = await service.GetTasksAsync(false, CancellationToken.None);

        Assert.Collection(
            tasks,
            task =>
            {
                Assert.Equal(1, task.Id);
                Assert.Equal("EF Coreを学ぶ", task.Title);
                Assert.Equal("学習", task.CategoryName);
            },
            task =>
            {
                Assert.Equal(3, task.Id);
                Assert.Equal("テストを読む", task.Title);
                Assert.Null(task.CategoryName);
            });
    }
}
