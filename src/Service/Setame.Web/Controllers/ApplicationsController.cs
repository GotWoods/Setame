using JasperFx.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setame.Data.Handlers.Applications;
using Application = Setame.Data.Models.Application;

namespace Setame.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(IMediator mediator, ILogger<ApplicationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application>>> GetApplications(CancellationToken ct)
    {
        var items = await _mediator.Send(new GetActiveApplications(), ct);
        _logger.LogDebug("Got {Count} applications", items.Count);
        var sorted = items.OrderBy(x => x.Name);
        return Ok(sorted);
    }

    [HttpGet("{applicationId}")]
    public async Task<ActionResult<Application>> GetApplication(Guid applicationId)
    {
        var application = await _mediator.Send(new GetApplication(applicationId));
        _logger.LogDebug("Application found by Id {ApplicationId}", applicationId);
        Response.TrySetETagResponseHeader(application.Version);
        return Ok(application);
    }

    [HttpPost]
    public async Task<IActionResult> CreateApplication(Application application, CancellationToken ct)
    {
        _logger.LogDebug("Creating a new application with name of {ApplicationName}", application.Name);
        var id = CombGuidIdGeneration.NewGuid();
        var result = await _mediator.Send(new CreateApplication(id, application.Name, application.Token, application.EnvironmentSetId), ct);
        return result.WasSuccessful ? Ok(id) : BadRequest(result);
    }

    [HttpDelete("{applicationId}")]
    public async Task<IActionResult> DeleteApplication(Guid applicationId)
    {
        await _mediator.Send(new DeleteApplication(applicationId));
        _logger.LogDebug("Deleted application {ApplicationId}", applicationId);
        return NoContent();
    }

    [HttpPut("{applicationId}/rename")]
    public async Task<IActionResult> RenameApplication(Guid applicationId, [FromBody] string newName, CancellationToken ct)
    {
        var version = Request.GetIfMatchRequestHeader();
        var result = await _mediator.Send(new RenameApplication(applicationId, version, newName));
        Response.TrySetETagResponseHeader(version + 1);
        _logger.LogDebug("Application {ApplicationId} renamed to {NewName}", applicationId, newName);
        return ControllerHelper.HttpResultFrom(result);
    }
}