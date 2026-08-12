namespace TaskApi.Models;

public sealed class TaskItem
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public bool IsCompleted { get; set; }

    public int? CategoryId { get; set; }

    public TaskCategory? Category { get; set; }
}
