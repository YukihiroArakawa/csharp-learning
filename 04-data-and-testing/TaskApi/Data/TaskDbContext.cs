using Microsoft.EntityFrameworkCore;
using TaskApi.Models;

namespace TaskApi.Data;

public sealed class TaskDbContext(DbContextOptions<TaskDbContext> options)
    : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public DbSet<TaskCategory> Categories => Set<TaskCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Property(task => task.Title)
                .HasMaxLength(100);

            entity.HasOne(task => task.Category)
                .WithMany(category => category.Tasks)
                .HasForeignKey(task => task.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<TaskCategory>(entity =>
        {
            entity.Property(category => category.Name)
                .HasMaxLength(50);
        });
    }
}
