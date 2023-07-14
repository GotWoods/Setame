using System.Security.Claims;
using System.Security;
using ConfigMan.Data;

namespace ConfigMan.Service
{
    public class ClaimsUserInfo : IUserInfo
    {
        private readonly IHttpContextAccessor _accessor;

        public ClaimsUserInfo(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public Guid GetCurrentUserId()
        {
            if (_accessor.HttpContext == null)
                throw new SecurityException("HttpContext is null");

            var claim = _accessor.HttpContext.User.FindFirst(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (claim == null)
                throw new SecurityException("User claim could not be found");
            return Guid.Parse(claim.Value);
        }
    }
}
