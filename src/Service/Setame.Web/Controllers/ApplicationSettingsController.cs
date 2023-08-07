using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setame.Data.Handlers;
using Setame.Data.Handlers.Applications;

namespace Setame.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationSettingsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ApplicationSettingsController> _logger;


    public ApplicationSettingsController(IMediator mediator, ILogger<ApplicationSettingsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Application")]
    public IActionResult Get()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
        {
            _logger.LogWarning("Can not find application @{Claims}", User.Claims);
            return NotFound("Claim not found");
        }

        return Ok();
    }

    [HttpPost("{applicationId}/{environment}")]
    public async Task<IActionResult> CreateNew(Guid applicationId, string environment, [FromBody] string variable, CancellationToken ct)
    {
        var version = Request.GetIfMatchRequestHeader();
        CommandResponse? response = null;
        if (environment == "default")
        {
            _logger.LogDebug("Adding default {Variable} to Application {ApplicationId}", variable, applicationId);
            response = await _mediator.Send(new CreateDefaultApplicationVariable(applicationId, version, variable), ct);
        }
        else
        {
            _logger.LogDebug("Adding {Environment} {Variable} to Application {ApplicationId}", environment, variable, applicationId);
            response = await _mediator.Send(new CreateApplicationVariable(applicationId, version, variable), ct);
        }
        Response.TrySetETagResponseHeader(response.NewVersion);
        return CreatedAtAction(nameof(CreateNew), null);
    }

    [HttpPut("{applicationId}/{environment}/{variable}")]
    public async Task<IActionResult> Update(Guid applicationId, string environment, string variable, [FromBody] string value, CancellationToken ct)
    {
        var version = Request.GetIfMatchRequestHeader();
        CommandResponse result = null!;
        if (environment == "default")
        {
            _logger.LogDebug("Updating default {Variable} for Application {ApplicationId}. Set to {Value}", variable, applicationId, value);
            result = await _mediator.Send(new UpdateDefaultApplicationVariable(applicationId, version, variable, value), ct);
        }
        else
        {
            _logger.LogDebug("Updating {Environment} {Variable} for Application {ApplicationId}. Set to {Value}", environment, variable, applicationId, value);
            result = await _mediator.Send(new UpdateApplicationVariable(applicationId, version, environment, variable, value), ct);
        }

        return ControllerHelper.HttpResultFrom(result, Response);
    }

    [HttpPost("{applicationId}/{variable}/rename")]
    public async Task<IActionResult> RenameVariable(Guid applicationId, string variable, [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        var result = await _mediator.Send(new RenameApplicationVariable(applicationId, version, variable, newName));
        if (result.WasSuccessful)
            _logger.LogDebug("For Application {ApplicationId}, {Variable} renamed to {NewName}", applicationId, variable, newName);
        return ControllerHelper.HttpResultFrom(result, Response);
    }

    [HttpDelete("{applicationId}/{variable}")]
    public async Task<IActionResult> DeleteVariable(Guid applicationId, string variable)
    {
        var version = Request.GetIfMatchRequestHeader();
        var result = await _mediator.Send(new DeleteApplicationVariable(applicationId, version, variable));
        return ControllerHelper.HttpResultFrom(result, Response);
    }

    [HttpPost("{applicationId}/{variable}/renameDefault")]
    public async Task<IActionResult> RenameDefaultVariable(Guid applicationId, string variable, [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        var result = await _mediator.Send(new RenameDefaultApplicationVariable(applicationId, version, variable, newName));
        if (result.WasSuccessful)
            _logger.LogDebug("For Application {ApplicationId}, default {Variable} renamed to {NewName}", applicationId, variable, newName);
        return ControllerHelper.HttpResultFrom(result, Response);
    }
}