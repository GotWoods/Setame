using ConfigMan.Data;
using ConfigMan.Data.Models;
using Marten;
using Marten.Linq;
using Marten.Schema.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using CreateEnvironmentSet = ConfigMan.Data.CreateEnvironmentSet;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EnvironmentSetsController : ControllerBase
{
    private readonly IDocumentSession _documentSession;
    private readonly IEnvironmentSetService _environmentSetService;
    private readonly IQuerySession _querySession;

    public EnvironmentSetsController(IEnvironmentSetService environmentSetService, IDocumentSession documentSession, IQuerySession querySession)
    {
        _environmentSetService = environmentSetService;
        _documentSession = documentSession;
        _querySession = querySession;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentSet>>> GetAll(CancellationToken ct)
    {
        //TODO: there has to be a better way of doing this
        var allIds = _querySession.Events.QueryAllRawEvents().Where(x => x.EventTypeName=="environment_set_created").Select(x=>x.StreamId).Distinct().ToList();

        var items = new List<EnvironmentSet>();
        foreach (var id in allIds)
        {
            items.Add(await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(id, token: ct));
        }
        
        var sorted = items.OrderBy(x => x.Name);
        return Ok(sorted);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<EnvironmentSet>> GetOne(string name)
    {
        var deploymentEnvironment = await _environmentSetService.GetOneAsync(name);
        return Ok(deploymentEnvironment);
    }

    [HttpPost]
    public async Task<ActionResult<EnvironmentSet>> Create(EnvironmentSet environmentSet, CancellationToken ct)
    {
        // var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        // if (claim == null)
        // {
        //     _logger.LogWarning("Can not find application @{Claims}", User.Claims);
        //     return NotFound("Claim not found");
        // }
        //User.Identity.Name
        var id = CombGuidIdGeneration.NewGuid();
        await _documentSession.Add<EnvironmentSet>(id, new CreateEnvironmentSet(id, environmentSet.Name), ct);
        //await _environmentSetService.CreateAsync(environmentSet);
        return NoContent();
    }
    
    [HttpPut("{name}")]
    public async Task<IActionResult> Update(string name, EnvironmentSet environmentSet)
    {
        await _environmentSetService.UpdateAsync(environmentSet);
        return NoContent();
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> Delete(string name)
    {
        await _environmentSetService.DeleteAsync(name);
        return NoContent();
    }
    
    [HttpPost("{name}/rename")]
    public async Task<IActionResult> RenameVariable(Guid Id, string name, [FromBody] string newName, CancellationToken ct)
    {
        //var version = HttpContext.GetIfMatchRequestHeader().GetSanitizedValue();
        var version = -1;
        await _documentSession.GetAndUpdate<EnvironmentSet>(Id, version, current => EnvironmentSetService.Handle(current, new RenameEnvironmentSet(name, newName)), ct);
        await _environmentSetService.RenameAsync(name, newName);
        return NoContent();
    }
    

    [HttpPost("{environmentId}/environment/{environmentName")]
    public async Task<IActionResult> AddEnvironment(Guid environmentId, string environmentName, CancellationToken ct)
    {
        await _documentSession.Add<EnvironmentSet>(environmentId, new EnvironmentAdded(environmentName), ct);
        return NoContent();
    }

    [HttpDelete("{environmentId}/environment/{environmentName")]
    public async Task<IActionResult> DeleteEnvironment(Guid environmentId, string environmentName, CancellationToken ct)
    {
        await _documentSession.Add<EnvironmentSet>(environmentId, new EnvironmentRemoved(environmentName), ct);
        return NoContent();
    }
}