using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Setame.Web;

namespace Setame.Tests.Web
{
    public class ETagExtensionsTests
    {
        [Fact]
        public void GetIfMatchRequestHeader_HeaderIsNull_ThrowsNullReferenceException()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => context.Request.GetIfMatchRequestHeader());
        }

        [Theory]
        [InlineData("\"123\"", 123)]
        [InlineData("\"456\"", 456)]
        public void GetIfMatchRequestHeader_ValidHeader_ReturnsParsedValue(string headerValue, int expected)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers[HeaderNames.IfMatch] = headerValue;

            // Act
            var result = context.Request.GetIfMatchRequestHeader();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TrySetETagResponseHeader_ResponseNotSuccessful_NoETagSet()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = StatusCodes.Status400BadRequest; // Make the response unsuccessful

            // Act
            context.Response.TrySetETagResponseHeader("123");

            // Assert
            var eTagHeader = context.Response.Headers[HeaderNames.ETag];
            Assert.True(string.IsNullOrEmpty(eTagHeader));
        }

        [Theory]
        [InlineData("123", "W/\"123\"")]
        [InlineData("456", "W/\"456\"")]
        public void TrySetETagResponseHeader_ResponseSuccessful_ETagSet(string etagValue, string expectedHeaderValue)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.StatusCode = StatusCodes.Status200OK; // Make the response successful

            // Act
            context.Response.TrySetETagResponseHeader(etagValue);

            // Assert
            var eTagHeader = context.Response.Headers[HeaderNames.ETag].ToString();
            Assert.Equal(expectedHeaderValue, eTagHeader);
        }
    }
}
