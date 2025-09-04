namespace LibrarySystemApplication.Test;

using Xunit;

public class SampleTests
{
    [Fact]
    public void AddingTwoNumbers_ShouldReturnCorrectResult()
    {
        // Arrange
        var a = 2;
        var b = 3;

        // Act
        var result = a + b;

        // Assert
        Assert.Equal(5, result);
    }
}
