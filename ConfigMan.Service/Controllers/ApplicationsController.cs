using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using JasperFx.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application>>> GetApplications(CancellationToken ct)
    {
        var items = await _mediator.Send(new GetActiveApplications(), ct);
        var sorted = items.OrderBy(x => x.Name);
        return Ok(sorted);
    }

    [HttpGet("{applicationId}")]
    public async Task<ActionResult<Application>> GetApplication(Guid applicationId)
    {
        return Ok(await _mediator.Send(new GetApplication(applicationId)));
    }

    [HttpPost]
    public async Task<IActionResult> CreateApplication(Application application, CancellationToken ct)
    {
        var id = CombGuidIdGeneration.NewGuid();
        await _mediator.Send(new CreateApplication(id, application.Name, application.Token, application.EnvironmentSetId));
        return NoContent();
    }

    [HttpDelete("{applicationId}")]
    public async Task<IActionResult> DeleteApplication(Guid applicationId)
    {
        await _mediator.Send(new DeleteApplication(applicationId));
        return NoContent();
    }
}