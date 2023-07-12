using System;
using System.Security.Claims;
using ConfigMan.Data.Handlers.Applications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationSettingsController : ControllerBase
{
    private readonly IMediator _mediator;
    

    public ApplicationSettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = "Application")]
    public async Task<ActionResult> Get()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
    //        _logger.LogWarning("Can not find application @{Claims}", User.Claims);
            return NotFound("Claim not found");
        }

        return Ok();
    }

    [HttpPost("{applicationId}/{environment}")]
    public async Task<ActionResult> CreateNew(Guid applicationId, string environment, [FromBody] string variable, CancellationToken ct)
    {
        var version = Request.GetIfMatchRequestHeader();
        if (environment == "default")
            await _mediator.Send(new CreateDefaultApplicationVariable(applicationId, variable));

        else
            await _mediator.Send(new CreateApplicationVariable(applicationId, environment, variable));
        Response.TrySetETagResponseHeader(version + 1);
        return CreatedAtAction(nameof(CreateNew), null);
    }

    [HttpPut("{applicationId}/{environment}/{variable}")]
    public async Task<ActionResult> Update(Guid applicationId, string environment, string variable, [FromBody] string value, CancellationToken ct)
    {
        if (applicationId == Guid.Empty)
            throw new ArgumentNullException("applicationId");
        if (string.IsNullOrEmpty(environment))
            throw new ArgumentNullException("environment");
        if (string.IsNullOrEmpty(variable))
            throw new ArgumentNullException("variable");
        if (string.IsNullOrEmpty(value))
            throw new ArgumentNullException("value");

        var version = Request.GetIfMatchRequestHeader();
        if (environment == "default")
            await _mediator.Send(new UpdateDefaultApplicationVariable(applicationId, version,variable, value));
        else
            await _mediator.Send(new UpdateApplicationVariable(applicationId, version, environment, variable, value));
        Response.TrySetETagResponseHeader(version + 1);
        return Ok();
    }

    [HttpPost("{applicationId}/{variable}/rename")]
    public async Task<IActionResult> RenameVariable(Guid applicationId, string variable, [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        await _mediator.Send(new RenameApplicationVariable(applicationId, version, variable, newName));
        Response.TrySetETagResponseHeader(version + 1);
        return Ok();
    }
}