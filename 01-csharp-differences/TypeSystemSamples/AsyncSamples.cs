public static class AsyncSamples
{
    public static async Task<string> ReadMessageAsync()
    {
        var path = Path.Combine(Path.GetTempPath(), "csharp-learning-async.txt");

        await File.WriteAllTextAsync(path, "Hello from async file I/O");
        return await File.ReadAllTextAsync(path);
    }
}
