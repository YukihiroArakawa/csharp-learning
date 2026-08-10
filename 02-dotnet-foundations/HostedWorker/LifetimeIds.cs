namespace HostedWorker;

public sealed class SingletonId
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class ScopedId
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class TransientId
{
    public Guid Id { get; } = Guid.NewGuid();
}
