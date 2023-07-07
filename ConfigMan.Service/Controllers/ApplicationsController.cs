using ConfigMan.Data;
using ConfigMan.Data.Models;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfigMan.Service.Controllers.MyApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;

    public ApplicationsController(IApplicationService applicationService, IDocumentSession documentSession, IQuerySession querySession)
    {
        _applicationService = applicationService;
        _documentSession = documentSession;
        _querySession = querySession;
    }

    // GET: api/Applications
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application>>> GetApplications(CancellationToken ct)
    {
        var allIds = _querySession.Events.QueryAllRawEvents().Where(x => x.EventTypeName == "application_created").Select(x => x.StreamId).Distinct().ToList();

        var items = new List<Application>();
        foreach (var id in allIds)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<Application>(id, token: ct);
            items.Add(aggregateStreamAsync);
        }

        var sorted = items.OrderBy(x => x.Name);
        return Ok(sorted);
    }


    [HttpGet("{applicationId}")]
    public async Task<ActionResult<Application>> GetApplication(Guid applicationId)
    {
        var app = await _querySession.Events.AggregateStreamAsync<Application>(applicationId);
        //var associations = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == app.EnvironmentSetId);
        //var application = await _applicationService.GetApplicationByIdAsync(name);
        return Ok(app);
    }

    // POST: api/Applications
    [HttpPost]
    public async Task<ActionResult<Application>> CreateApplication(Application application, CancellationToken ct)
    {
        var environment = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(application.EnvironmentSetId);

        var id = CombGuidIdGeneration.NewGuid();
        await _documentSession.Add<Application>(id, new ApplicationCreated(id, application.Name, application.Token, application.EnvironmentSetId), User, ct);
        foreach (var deploymentEnvironment in environment.DeploymentEnvironments)
        {
            await _documentSession.GetAndUpdate<Application>(id, -1, x => new ApplicationEnvironmentAdded(deploymentEnvironment.Name), User, ct);
            await _documentSession.GetAndUpdate<EnvironmentSet>(application.EnvironmentSetId, -1, x => new ApplicationAssociatedToEnvironmentSet(id, application.EnvironmentSetId), User, ct);
        }

        await _documentSession.SaveChangesAsync(ct);
        return NoContent();
    }

    // DELETE: api/Applications/5
    [HttpDelete("{applicationId}")]
    public async Task<IActionResult> DeleteApplication(Guid applicationId)
    {
        _documentSession.Delete<Application>(applicationId);
        await _documentSession.SaveChangesAsync();
        //await _applicationService.DeleteApplicationAsync(name);
        return NoContent();
    }
}