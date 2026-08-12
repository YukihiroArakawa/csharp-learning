namespace TaskApi.Models;

public sealed record TaskSummary(
    int Id,
    string Title,
    bool IsCompleted,
    string? CategoryName);
