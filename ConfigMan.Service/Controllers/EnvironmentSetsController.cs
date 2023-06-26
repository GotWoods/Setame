using ConfigMan.Data;
using ConfigMan.Data.Models;
using Marten;
using Marten.Linq;
using Marten.Schema.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

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
        //This is missing deleted items
        var allIds = _querySession.Events.QueryAllRawEvents().Where(x => x.EventTypeName=="environment_set_created").Select(x=>x.StreamId).Distinct().ToList();

        var items = new List<EnvironmentSet>();
        foreach (var id in allIds)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(id, token: ct);
            items.Add(aggregateStreamAsync);
        }
        
        var sorted = items.OrderBy(x => x.Name);
        return Ok(sorted);
    }

    // [HttpGet("{environmentId}")]
    // public async Task<ActionResult<EnvironmentSet>> GetOne(Guid environmentId)
    // {
    //     var deploymentEnvironment = await _environmentSetService.GetOneAsync(name);
    //     return Ok(deploymentEnvironment);
    // }

    [HttpPost]
    public async Task<ActionResult<EnvironmentSet>> Create(EnvironmentSet environmentSet, CancellationToken ct)
    {
        //var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        // if (claim == null)
        // {
        //     _logger.LogWarning("Can not find application @{Claims}", User.Claims);
        //     return NotFound("Claim not found");
        // }
        //User.Identity.Name
        var id = CombGuidIdGeneration.NewGuid();
        await _documentSession.Add<EnvironmentSet>(id, new EnvironmentSetCreated(id, environmentSet.Name), User, ct);
        //await _environmentSetService.CreateAsync(environmentSet);
        return NoContent();
    }
    
    // [HttpPut("{name}")]
    // public async Task<IActionResult> Update(string name, EnvironmentSet environmentSet)
    // {
    //     await _environmentSetService.UpdateAsync(environmentSet);
    //     return NoContent();
    // }

    [HttpDelete("{environmentId}")]
    public async Task<IActionResult> Delete(Guid environmentId, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x => new EnvironmentSetDeleted(), User, ct);
        _documentSession.Delete<EnvironmentSet>(environmentId);
        await _documentSession.SaveChangesAsync(ct);
        //await _environmentSetService.DeleteAsync(name);
        return NoContent();
    }
    
    [HttpPost("{environmentId}/rename")]
    public async Task<IActionResult> RenameVariable(Guid environmentId, [FromBody] string newName, CancellationToken ct)
    {
        //var version = HttpContext.GetIfMatchRequestHeader().GetSanitizedValue();
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, z => new EnvironmentSetRenamed(newName), User, ct);
        //await _environmentSetService.RenameAsync(name, newName);
        return NoContent();
    }
    
    [HttpPost("{environmentId}/environment")]
    public async Task<IActionResult> AddEnvironment(Guid environmentId, [FromBody] string environmentName, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1,  x=> new EnvironmentAdded(environmentName), User,ct);
        await _documentSession.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{environmentId}/environment/{environmentName}")]
    public async Task<IActionResult> DeleteEnvironment(Guid environmentId, string environmentName, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x=>new EnvironmentRemoved(environmentName), User, ct);
        return NoContent();
    }
    
    [HttpPost("{environmentId}/variable")]
    public async Task<IActionResult> AddVariable(Guid environmentId, [FromBody] string variableName, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x => new EnvironmentSetVariableAdded(variableName), User, ct);
        await _documentSession.SaveChangesAsync(ct);
        return NoContent();
    }
}