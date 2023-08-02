using System.Security;
using Org.BouncyCastle.Asn1.Ocsp;
using Setame.Data;

namespace Setame.Web;

public class WebGetCallbackUrl : ICallbackUrlProvider
{
    private readonly IHttpContextAccessor _accessor;

    public WebGetCallbackUrl(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
    public string GetCallbackUrl()
    {
        if (_accessor.HttpContext == null)
            throw new SecurityException("HttpContext is null");

        var request = _accessor.HttpContext.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}