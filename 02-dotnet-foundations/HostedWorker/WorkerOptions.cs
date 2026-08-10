namespace HostedWorker;

public sealed class WorkerOptions
{
    public const string SectionName = "Worker";

    public string Message { get; init; } = string.Empty;

    public int DelayMilliseconds { get; init; }
}
