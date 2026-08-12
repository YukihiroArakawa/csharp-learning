namespace TaskApi.Models;

public sealed class TaskCategory
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<TaskItem> Tasks { get; } = new List<TaskItem>();
}
