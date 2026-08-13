namespace TaskManagementApi.Models;

public sealed record TaskSummary(
    int Id,
    string Title,
    bool IsCompleted);
