namespace CSharpDifferences.Tests;

public class UnitTest1
{
    [Fact]
    public void Describe_returns_label_for_negative_integer()
    {
        var actual = PatternSamples.Describe(-1);

        Assert.Equal("negative number: -1", actual);

    }
}
