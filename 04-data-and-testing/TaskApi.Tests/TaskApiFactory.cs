using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskApi.Data;

namespace TaskApi.Tests;

public sealed class TaskApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection;

    public TaskApiFactory()
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            connection.Dispose();
        }
    }
}
