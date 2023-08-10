using System.Security;
using Microsoft.AspNetCore.Http;
using Moq;
using Setame.Web;

namespace Setame.Tests.Web;

public class WebGetCallbackUrlTests
{
    [Fact]
    public void GetCallbackUrl_HttpContextIsNull_ThrowsSecurityException()
    {
        // Arrange
        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns((HttpContext)null);

        var subject = new WebGetCallbackUrl(mockAccessor.Object);

        // Act & Assert
        Assert.Throws<SecurityException>(() => subject.GetCallbackUrl());
    }

    [Theory]
    [InlineData("https", "example.com", "https://example.com")]
    [InlineData("http", "localhost:5000", "http://localhost:5000")]
    public void GetCallbackUrl_ValidHttpContext_ReturnsExpectedUrl(string scheme, string host, string expectedUrl)
    {
        // Arrange
        var mockRequest = new Mock<HttpRequest>();
        mockRequest.SetupGet(r => r.Scheme).Returns(scheme);
        mockRequest.SetupGet(r => r.Host).Returns(new HostString(host));

        var mockContext = new Mock<HttpContext>();
        mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);

        var mockAccessor = new Mock<IHttpContextAccessor>();
        mockAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);

        var subject = new WebGetCallbackUrl(mockAccessor.Object);

        // Act
        var result = subject.GetCallbackUrl();

        // Assert
        Assert.Equal(expectedUrl, result);
    }
}