namespace TaskManagementApi.Models;

public sealed record UpdateTaskRequest(
    string? Title,
    bool IsCompleted);
