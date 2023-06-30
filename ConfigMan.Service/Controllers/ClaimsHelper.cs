using System.Security;
using Marten;
using System.Security.Claims;

namespace ConfigMan.Service.Controllers
{
    public class ClaimsHelper
    {
        public static Guid GetCurrentUserId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (claim == null)
                throw new SecurityException("User claim could not be found");
            return Guid.Parse(claim.Value);
        }
    }
}
