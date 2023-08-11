using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Setame.Data.Projections;

namespace Setame.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Application")]
public class ClientController : ControllerBase
{
    private readonly IQuerySession _querySession;

    public ClientController(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    [HttpGet]
   public IActionResult Get()
    {
        var appId = User.FindFirst(ClaimTypes.NameIdentifier);
        if (appId == null)
        {
            return NotFound("Claim not found");
        }

        var env = User.FindFirst(ClaimTypes.Actor);
        if (env == null)
        {
            return NotFound("Claim not found");
        }

        var settings = _querySession.Query<ApplicationSettings>().FirstOrDefault(x => x.Id == Guid.Parse(appId.Value));
        return Ok(settings.Settings.GetVariablesForEnvironment(env.Value));

    }
}