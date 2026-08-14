namespace TaskManagementApi.Models;

public sealed class TaskItem
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public bool IsCompleted { get; set; }
}
