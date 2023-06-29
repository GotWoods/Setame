using ConfigMan.Data.Models;
using ConfigMan.Data.Models.Projections;
using Marten;
using Marten.Schema.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EnvironmentSetsController : ControllerBase
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;
    private readonly IDocumentStore _documentStore;

    public EnvironmentSetsController(IDocumentSession documentSession, IQuerySession querySession, IDocumentStore documentStore)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _documentStore = documentStore;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnvironmentSet>>> GetAll(CancellationToken ct)
    {
    //    _querySession.Streams
        //TODO: there has to be a better way of doing this
        //This is missing deleted items
        var allIds = _querySession.Events.QueryAllRawEvents().Where(x => x.EventTypeName == "environment_set_created").Select(x => x.StreamId).Distinct().ToList();

        var items = new List<EnvironmentSet>();
        foreach (var id in allIds)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(id, token: ct);
            items.Add(aggregateStreamAsync);
        }

        var sorted = items.OrderBy(x => x.Name);
        return Ok(sorted);
    }

    [HttpGet("{environmentId}")]
    public async Task<ActionResult<EnvironmentSet>> GetOne(Guid environmentId, CancellationToken ct)
    {
        var deploymentEnvironment = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(environmentId, token: ct);
        return Ok(deploymentEnvironment);
    }

    [HttpPost]
    public async Task<ActionResult<EnvironmentSet>> Create(EnvironmentSet environmentSet, CancellationToken ct)
    {
        // var deamon = await _documentStore.BuildProjectionDaemonAsync();
        // await deamon.RebuildProjection<EnvironmentSetSummary>(ct);


        var id = CombGuidIdGeneration.NewGuid();
        await _documentSession.Add<EnvironmentSet>(id, new EnvironmentSetCreated(id, environmentSet.Name), User, ct);
        return NoContent();
    }

    [HttpDelete("{environmentId}")]
    public async Task<IActionResult> Delete(Guid environmentId, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x => new EnvironmentSetDeleted(environmentId), User, ct);
        _documentSession.Delete<EnvironmentSet>(environmentId);
        await _documentSession.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{environmentId}/rename")]
    public async Task<IActionResult> RenameEnvironmentSet(Guid environmentId, [FromBody] string newName, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, z => new EnvironmentSetRenamed(environmentId, newName), User, ct);
        return NoContent();
    }

    [HttpPost("{environmentId}/environment")]
    public async Task<IActionResult> AddEnvironment(Guid environmentId, [FromBody] string environmentName, CancellationToken ct)
    {
        //TOOD: when a new environment is added, it should go to the children that use that environment set as well
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x => new EnvironmentAdded(environmentName), User, ct);
        await _documentSession.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete("{environmentId}/environment/{environmentName}")]
    public async Task<IActionResult> DeleteEnvironment(Guid environmentId, string environmentName, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x => new EnvironmentRemoved(environmentName), User, ct);
        return NoContent();
    }

    [HttpPost("{environmentId}/variable")]
    public async Task<IActionResult> AddVariable(Guid environmentId, [FromBody] string variableName, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x => new EnvironmentSetVariableAdded(variableName), User, ct);
        await _documentSession.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPut("{environmentId}/variable/{environment}/{variableName}")]
    public async Task<IActionResult> UpdateVariable(Guid environmentId, string environment, string variableName, [FromBody] string variableValue, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x => new EnvironmentSetVariableChanged(environment, variableName, variableValue), User, ct);
        await _documentSession.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPut("{environmentId}/variable/{variableName}/rename")]
    public async Task<IActionResult> RenameVariable(Guid environmentId, string variableName, [FromBody] string newName, CancellationToken ct)
    {
        await _documentSession.GetAndUpdate<EnvironmentSet>(environmentId, -1, x => new EnvironmentSetVariableRenamed(variableName, newName), User, ct);
        await _documentSession.SaveChangesAsync(ct);
        return NoContent();
    }
}