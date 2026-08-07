public sealed class SyncResource(String name) : IDisposable
{
    public void Use()
    {
        Console.WriteLine($"using {name}");
    }
    public void Dispose()
    {
        Console.WriteLine($"disposing {name}");
    }
}

public sealed class AsyncResource(string name) : IAsyncDisposable
{
    public void Use()
    {
        Console.WriteLine($"using {name}");
    }

    public ValueTask DisposeAsync()
    {
        Console.WriteLine($"async disposing {name}");
        return ValueTask.CompletedTask;
    }
}
