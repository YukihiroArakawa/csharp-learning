public static class PatternSamples
{
    public static string Describe(object? value)
    {
        return value switch
        {
            null => "null",
            int number when number < 0 => $"negative number: {number}",
            int number => $"number: {number}",
            string text => $"text: {text}",
            _ => "other",
        };
    }
}
