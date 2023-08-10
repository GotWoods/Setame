using System.Security;
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