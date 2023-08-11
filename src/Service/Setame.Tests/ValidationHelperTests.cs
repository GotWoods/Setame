using Setame.Data;

namespace Setame.Tests;

public class ValidationHelperTests
{
    [Theory]
    [InlineData("HelloWorld")]  // Letters
    [InlineData("Hello123")]    // Numbers
    [InlineData("Hello.World")] // Containing "."
    [InlineData("Hello'World")] // Single quote
    [InlineData("Hello\"World")]// Double quote
    [InlineData("123.456")]     // Numbers with "."
    public void BeValidString_ShouldReturnTrue_ForValidStrings(string value)
    {
        // Act
        var isValid = ValidationHelper.BeValidString(value);

        // Assert
        Assert.True(isValid);
    }

    [Theory]
    [InlineData("Hello\nWorld")] // Invalid character: "\n"
    public void BeValidString_ShouldReturnFalse_ForInvalidStrings(string value)
    {
        // Act
        var isValid = ValidationHelper.BeValidString(value);

        // Assert
        Assert.False(isValid);
    }
}