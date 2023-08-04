using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Setame.Data.Handlers.EnvironmentSets;
using Setame.Data.Models;

namespace Setame.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EnvironmentSetsController : ControllerBase
{
    private readonly ILogger<EnvironmentSetsController> _logger;
    private readonly IMediator _mediator;

    public EnvironmentSetsController(IMediator mediator, ILogger<EnvironmentSetsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentSet>>> GetAll()
    {
        _logger.LogDebug("Getting all");
        var results = await _mediator.Send(new GetActiveEnvironmentSets());
        _logger.LogDebug("Found {Count} Environment Sets", results.Count);
        return Ok(results);
    }

    [HttpGet("{environmentSetId}")]
    public async Task<ActionResult<EnvironmentSet>> GetOne(Guid environmentSetId)
    {
        _logger.LogDebug("Getting Environment Set {Id}", environmentSetId);
        var deploymentEnvironment = await _mediator.Send(new GetEnvironment(environmentSetId));
        Response.TrySetETagResponseHeader(deploymentEnvironment.Version.ToString());
        _logger.LogDebug("Found Environment Set");
        return Ok(deploymentEnvironment);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] string name)
    {
        _logger.LogDebug("Creating a new environment set {Name}", name);
        var result = await _mediator.Send(new CreateEnvironmentSet(name));
        if (!result.WasSuccessful)
        {
            _logger.LogWarning("Creation failed. {@Errors}", result.Errors);
            return new BadRequestObjectResult(result);
        }

        _logger.LogDebug("Environment Set Created. New id is {Id}", result.Data);
        return Ok(result.Data);
    }

    [HttpDelete("{environmentSetId}")]
    public async Task<IActionResult> Delete(Guid environmentSetId)
    {
        var result = await _mediator.Send(new DeleteEnvironmentSet(environmentSetId));
        if (result.WasSuccessful)
            _logger.LogDebug("Environment set {Id} was deleted", environmentSetId);
        return ControllerHelper.HttpResultFrom(result, Response);
    }

    [HttpPut("{environmentSetId}/rename")]
    public async Task<IActionResult> RenameEnvironmentSet(Guid environmentSetId, [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        var response = await _mediator.Send(new RenameEnvironmentSet(environmentSetId, version, newName));
        if (response.WasSuccessful) 
            _logger.LogDebug("Environment set {Id} was renamed to {NewName}", environmentSetId, newName);
        return ControllerHelper.HttpResultFrom(response, Response);
    }


    [HttpPost("{environmentSetId}/environment")]
    public async Task<IActionResult> AddEnvironment(Guid environmentSetId, [FromBody] string environmentName)
    {
        var version = Request.GetIfMatchRequestHeader();
        var response = await _mediator.Send(new AddEnvironmentToEnvironmentSet(environmentSetId, version, environmentName));
        if (response.WasSuccessful) 
            _logger.LogDebug("Environment set {Id} added a new environment named {EnvironmentName}", environmentSetId, environmentName);
        return ControllerHelper.HttpResultFrom(response, Response);
    }

    [HttpDelete("{environmentSetId}/environment/{environmentName}")]
    public async Task<IActionResult> DeleteEnvironment(Guid environmentSetId, string environmentName)
    {
        var version = Request.GetIfMatchRequestHeader();
        var response = await _mediator.Send(new DeleteEnvironmentFromEnvironmentSet(environmentSetId, version, environmentName));
        if (response.WasSuccessful) 
            _logger.LogDebug("Environment set {Id} had the environment {EnvironmentName} removed", environmentSetId, environmentName);
        return ControllerHelper.HttpResultFrom(response, Response);
    }

    [HttpPut("{environmentSetId}/environment/{environmentName}/rename")]
    public async Task<IActionResult> RenameEnvironment(Guid environmentSetId, string environmentName,
        [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        var response = await _mediator.Send(new RenameEnvironment(environmentSetId, version, environmentName, newName));
        if (response.WasSuccessful) 
            _logger.LogDebug("Environment set {Id} had the environment {OldName} renamed to {NewName}", environmentSetId, environmentName, newName);
        return ControllerHelper.HttpResultFrom(response, Response);
    }

    [HttpPost("{environmentSetId}/variable")]
    public async Task<IActionResult> AddVariable(Guid environmentSetId, [FromBody] string variableName)
    {
        var version = Request.GetIfMatchRequestHeader();
        var response = await _mediator.Send(new AddVariableToEnvironmentSet(environmentSetId, version, variableName));
        if (response.WasSuccessful)
            _logger.LogDebug("Environment set {Id} had the {VariableName} variable added", environmentSetId, variableName);
        return ControllerHelper.HttpResultFrom(response, Response);
    }

    [HttpPut("{environmentSetId}/variable/{environment}/{variableName}")]
    public async Task<IActionResult> UpdateVariable(Guid environmentSetId, string environment, string variableName,
        [FromBody] string variableValue)
    {
        var version = Request.GetIfMatchRequestHeader();
        var response  = await _mediator.Send(new UpdateEnvironmentSetVariable(environmentSetId, version, environment, variableName, variableValue));
        if (response.WasSuccessful)
            _logger.LogDebug("Environment set {Id} had the {VariableName} set to {VariableValue} for environment {Environment}", environmentSetId, variableName, variableValue, environment);
        return ControllerHelper.HttpResultFrom(response, Response);
    }

    [HttpPut("{environmentSetId}/variable/{variableName}/rename")]
    public async Task<IActionResult> RenameVariable(Guid environmentSetId, string variableName,
        [FromBody] string newName)
    {
        var version = Request.GetIfMatchRequestHeader();
        var response = await _mediator.Send(new RenameEnvironmentSetVariable(environmentSetId, version, variableName, newName));
        if (response.WasSuccessful)
            _logger.LogDebug("Environment set {Id} had the {VariableName} variable renamed to {NewName}", environmentSetId, variableName, newName);
        return ControllerHelper.HttpResultFrom(response, Response);
    }
}