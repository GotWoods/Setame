using Xunit;
using Moq;
using System.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Setame.Web;

namespace Setame.Tests.Web
{
    public class ClaimsUserInfoTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public ClaimsUserInfoTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public void GetCurrentUserId_HttpContextIsNull_ThrowsSecurityException()
        {
            _mockHttpContextAccessor.Setup(a => a.HttpContext).Returns((HttpContext)null);

            var subject = new ClaimsUserInfo(_mockHttpContextAccessor.Object);

            Assert.Throws<SecurityException>(() => subject.GetCurrentUserId());
        }

        [Fact]
        public void GetCurrentUserId_ClaimNotFound_ThrowsSecurityException()
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _mockHttpContextAccessor.Setup(a => a.HttpContext.User).Returns(claimsPrincipal);

            var subject = new ClaimsUserInfo(_mockHttpContextAccessor.Object);

            Assert.Throws<SecurityException>(() => subject.GetCurrentUserId());
        }

        [Fact]
        public void GetCurrentUserId_ValidClaim_ReturnsUserId()
        {
            var userId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            _mockHttpContextAccessor.Setup(a => a.HttpContext.User).Returns(claimsPrincipal);

            var subject = new ClaimsUserInfo(_mockHttpContextAccessor.Object);

            var result = subject.GetCurrentUserId();

            Assert.Equal(userId, result);
        }
    }
}
