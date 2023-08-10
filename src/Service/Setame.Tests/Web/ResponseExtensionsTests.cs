using Microsoft.AspNetCore.Http;
using Moq;
using Setame.Web;

namespace Setame.Tests.Web;

public class ResponseExtensionsTests
{
    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(299)]
    public void IsSuccessful_HttpResponse_SuccessfulStatusCode_ReturnsTrue(int statusCode)
    {
        // Arrange
        var mockResponse = new Mock<HttpResponse>();
        mockResponse.SetupGet(r => r.StatusCode).Returns(statusCode);

        // Act
        var result = mockResponse.Object.IsSuccessful();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(199)]
    [InlineData(300)]
    [InlineData(404)]
    [InlineData(500)]
    public void IsSuccessful_HttpResponse_NonSuccessfulStatusCode_ReturnsFalse(int statusCode)
    {
        // Arrange
        var mockResponse = new Mock<HttpResponse>();
        mockResponse.SetupGet(r => r.StatusCode).Returns(statusCode);

        // Act
        var result = mockResponse.Object.IsSuccessful();

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(200)]
    [InlineData(201)]
    [InlineData(299)]
    public void IsSuccessful_HttpContext_SuccessfulStatusCode_ReturnsTrue(int statusCode)
    {
        // Arrange
        var mockResponse = new Mock<HttpResponse>();
        mockResponse.SetupGet(r => r.StatusCode).Returns(statusCode);

        var mockContext = new Mock<HttpContext>();
        mockContext.SetupGet(c => c.Response).Returns(mockResponse.Object);

        // Act
        var result = mockContext.Object.IsSuccessful();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(199)]
    [InlineData(300)]
    [InlineData(404)]
    [InlineData(500)]
    public void IsSuccessful_HttpContext_NonSuccessfulStatusCode_ReturnsFalse(int statusCode)
    {
        // Arrange
        var mockResponse = new Mock<HttpResponse>();
        mockResponse.SetupGet(r => r.StatusCode).Returns(statusCode);

        var mockContext = new Mock<HttpContext>();
        mockContext.SetupGet(c => c.Response).Returns(mockResponse.Object);

        // Act
        var result = mockContext.Object.IsSuccessful();

        // Assert
        Assert.False(result);
    }
}
