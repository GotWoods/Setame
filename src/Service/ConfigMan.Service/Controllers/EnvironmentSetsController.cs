using ConfigMan.Data.Handlers.EnvironmentSets;
using ConfigMan.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EnvironmentSetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EnvironmentSetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentSet>>> GetAll()
    {
        var results = await _mediator.Send(new GetActiveEnvironmentSets());
        return Ok(results);
    }

    [HttpGet("{environmentSetId}")]
    public async Task<ActionResult<EnvironmentSet>> GetOne(Guid environmentSetId)
    {
        var deploymentEnvironment = await _mediator.Send(new GetEnvironment(environmentSetId));
        Response.TrySetETagResponseHeader(deploymentEnvironment.Version.ToString());
        return Ok(deploymentEnvironment);
    }

    [HttpPost]
    public async Task<ActionResult> Create(EnvironmentSet environmentSet)
    {
        var result = await _mediator.Send(new CreateEnvironmentSet(environmentSet.Name));
        if (!result.WasSuccessful) 
            return new BadRequestObjectResult(result);
        return Ok(result.Data);
    }

    [HttpDelete("{environmentSetId}")]
    public async Task<IActionResult> Delete(Guid environmentSetId)
    {
        await _mediator.Send(new DeleteEnvironmentSet(environmentSetId));
        return NoContent();
    }

    [HttpPut("{environmentSetId}/rename")]
    public async Task<IActionResult> RenameEnvironmentSet(Guid environmentSetId, [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        await _mediator.Send(new RenameEnvironmentSet(environmentSetId, version, newName));
        Response.TrySetETagResponseHeader(version+1);
        return NoContent();
    }


    [HttpPost("{environmentSetId}/environment")]
    public async Task<IActionResult> AddEnvironment(Guid environmentSetId, [FromBody] string environmentName)
    {
        var version = Request.GetIfMatchRequestHeader();
        await _mediator.Send(new AddEnvironmentToEnvironmentSet(environmentSetId, version, environmentName));
        Response.TrySetETagResponseHeader(version + 1);
        return NoContent();
    }

    [HttpDelete("{environmentSetId}/environment/{environmentName}")]
    public async Task<IActionResult> DeleteEnvironment(Guid environmentSetId, string environmentName)
    {
        var version = Request.GetIfMatchRequestHeader();
        await _mediator.Send(new DeleteEnvironmentFromEnvironmentSet(environmentSetId, version, environmentName));
        return NoContent();
    }

    [HttpPut("{environmentSetId}/environment/{environmentName}/rename")]
    public async Task<IActionResult> RenameEnvironment(Guid environmentSetId, string environmentName, [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        await _mediator.Send(new RenameEnvironment(environmentSetId, version, environmentName, newName));
        Response.TrySetETagResponseHeader(version + 1);
        return NoContent();
    }

    [HttpPost("{environmentSetId}/variable")]
    public async Task<IActionResult> AddVariable(Guid environmentSetId, [FromBody] string variableName)
    {
        var version = Request.GetIfMatchRequestHeader();
        await _mediator.Send(new AddVariableToEnvironmentSet(environmentSetId, version, variableName));
        Response.TrySetETagResponseHeader(version + 1);
        return NoContent();
    }

    [HttpPut("{environmentSetId}/variable/{environment}/{variableName}")]
    public async Task<IActionResult> UpdateVariable(Guid environmentSetId, string environment, string variableName, [FromBody] string variableValue)
    {
        var version = Request.GetIfMatchRequestHeader();
        await _mediator.Send(new UpdateEnvironmentSetVariable(environmentSetId, version, environment, variableName, variableValue));
        Response.TrySetETagResponseHeader(version + 1);
        return NoContent();
    }

    [HttpPut("{environmentSetId}/variable/{variableName}/rename")]
    public async Task<IActionResult> RenameVariable(Guid environmentSetId, string variableName, [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        await _mediator.Send(new RenameEnvironmentSetVariable(environmentSetId, version, variableName, newName));
        Response.TrySetETagResponseHeader(version + 1);
        return NoContent();
    }
}