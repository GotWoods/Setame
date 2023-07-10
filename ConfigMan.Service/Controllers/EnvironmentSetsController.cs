using ConfigMan.Data.Handlers;
using ConfigMan.Data.Handlers.EnvironmentSets;
using ConfigMan.Data.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EnvironmentSetsController : ControllerBase
{
 private readonly IEnvironmentSetService _environmentSetService;
 private readonly IMediator _mediator;

 public EnvironmentSetsController(IEnvironmentSetService environmentSetService, IMediator mediator)
 {
     _environmentSetService = environmentSetService;
     _mediator = mediator;
 }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentSet>>> GetAll()
    {
        return Ok(await _environmentSetService.GetAll());
    }

    [HttpGet("{environmentSetId}")]
    public async Task<ActionResult<EnvironmentSet>> GetOne(Guid environmentSetId)
    {
        var deploymentEnvironment = await _environmentSetService.GetOne(environmentSetId);
        return Ok(deploymentEnvironment);
    }

    [HttpPost]
    public async Task<ActionResult> Create(EnvironmentSet environmentSet)
    {
        await _mediator.Send(new CreateEnvironmentSet(environmentSet.Name, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }

    [HttpDelete("{environmentSetId}")]
    public async Task<IActionResult> Delete(Guid environmentSetId)
    {
        await _mediator.Send(new DeleteEnvironmentSet(environmentSetId, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }

    [HttpPut("{environmentSetId}/rename")]
    public async Task<IActionResult> RenameEnvironmentSet(Guid environmentSetId, [FromBody] string newName)
    {
        await _mediator.Send(new RenameEnvironmentSet(environmentSetId, newName, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }


    [HttpPost("{environmentSetId}/environment")]
    public async Task<IActionResult> AddEnvironment(Guid environmentSetId, [FromBody] string environmentName)
    {
        await _mediator.Send(new AddEnvironmentToEnvironmentSet(environmentSetId, environmentName, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }

    [HttpDelete("{environmentSetId}/environment/{environmentName}")]
    public async Task<IActionResult> DeleteEnvironment(Guid environmentSetId, string environmentName)
    {
        await _mediator.Send(new DeleteEnvironmentFromEnvironmentSet(environmentSetId, environmentName, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }

    [HttpPut("{environmentSetId}/environment/{environmentName}/rename")]
    public async Task<IActionResult> RenameEnvironment(Guid environmentSetId, string environmentName, [FromBody] string newName)
    {
        await _mediator.Send(new RenameEnvironment(environmentSetId, environmentName, newName, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }

    [HttpPost("{environmentSetId}/variable")]
    public async Task<IActionResult> AddVariable(Guid environmentSetId, [FromBody] string variableName)
    {
        await _environmentSetService.Handle(new AddVariableToEnvironmentSet(environmentSetId, variableName, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }

    [HttpPut("{environmentSetId}/variable/{environment}/{variableName}")]
    public async Task<IActionResult> UpdateVariable(Guid environmentSetId, string environment, string variableName, [FromBody] string variableValue)
    {
        await _environmentSetService.Handle(new UpdateEnvironmentSetVariable(environmentSetId, environment, variableName, variableValue, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }

    [HttpPut("{environmentSetId}/variable/{variableName}/rename")]
    public async Task<IActionResult> RenameVariable(Guid environmentSetId, string variableName, [FromBody] string newName)
    {
        await _environmentSetService.Handle(new RenameEnvironmentSetVariable(environmentSetId, variableName, newName, ClaimsHelper.GetCurrentUserId(User)));
        return NoContent();
    }
}