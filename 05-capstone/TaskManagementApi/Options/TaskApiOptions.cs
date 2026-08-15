namespace TaskManagementApi.Options;

public sealed class TaskApiOptions
{
    public const string SectionName = "TaskApi";

    public int DefaultPageSize { get; init; }

    public int MaxPageSize { get; init; }
}
