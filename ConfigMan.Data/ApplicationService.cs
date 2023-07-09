using ConfigMan.Data.Models;
using Marten;


namespace ConfigMan.Data;

public record DeleteApplication(Guid ApplicationId, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record CreateApplication(Guid ApplicationId, string Name, string Token, Guid EnvironmentSetId, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public interface IApplicationService
{
    Task<IList<Application>> GetAll();
    Task<Application> GetOne(Guid applicationId);

    // Task<IEnumerable<Application>> GetApplicationsAsync();
    // Task<Application?> GetApplicationByIdAsync(string name);
    // Task<Application> CreateApplicationAsync(Application application);
    // Task UpdateApplicationAsync(Application application);
    // Task DeleteApplicationAsync(string name);

    Task Handle(DeleteApplication command);
    Task Handle(CreateApplication command);
}

public class ApplicationService : ServiceBase, IApplicationService
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;

    public ApplicationService(IDocumentSession documentSession, IQuerySession querySession) : base(documentSession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task<IList<Application>> GetAll()
    {
        var allIds = _querySession.Events.QueryAllRawEvents().Where(x => x.EventTypeName == "application_created").Select(x => x.StreamId).Distinct().ToList();
        var items = new List<Application>();
        foreach (var id in allIds)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<Application>(id);
            items.Add(aggregateStreamAsync);
        }

        return items;
    }

    public async Task<Application> GetOne(Guid applicationId)
    {
        var app = await _querySession.Events.AggregateStreamAsync<Application>(applicationId);
        //var associations = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == app.EnvironmentSetId);
        //var application = await _applicationService.GetApplicationByIdAsync(name);
        return app;
    }


    // public async Task Create(Application application)
    // {
    //     var environment = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(application.EnvironmentSetId);
    //
    //     var id = CombGuidIdGeneration.NewGuid();
    //     await _documentSession.Add<Application>(id, new ApplicationCreated(id, application.Name, application.Token, application.EnvironmentSetId), User);
    //     foreach (var deploymentEnvironment in environment.DeploymentEnvironments)
    //     {
    //         await _documentSession.GetAndUpdate<Application>(id, -1, x => new ApplicationEnvironmentAdded(deploymentEnvironment.Name), User, ct);
    //         await _documentSession.GetAndUpdate<EnvironmentSet>(application.EnvironmentSetId, -1, x => new ApplicationAssociatedToEnvironmentSet(id, application.EnvironmentSetId), User, ct);
    //     }
    //
    //     await _documentSession.SaveChangesAsync(ct);
    //     return NoContent();
    // }

    public async Task Handle(DeleteApplication command)
    {
        await AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationDeleted(command.ApplicationId), command.PerformedBy);
        // _documentSession.Delete<Application>(e.ApplicationId);
        // await _documentSession.SaveChangesAsync();
        //await _applicationService.DeleteApplicationAsync(name);
        //return NoContent();
    }

    public async Task Handle(CreateApplication command)
    {
        var environment = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(command.EnvironmentSetId);
        _documentSession.Events.StartStream<Application>(command.ApplicationId, new ApplicationCreated(command.ApplicationId, command.Name, command.Token, command.EnvironmentSetId));
        foreach (var deploymentEnvironment in environment.DeploymentEnvironments)
        {
            await AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationEnvironmentAdded(deploymentEnvironment.Name), command.PerformedBy);
            await AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new ApplicationAssociatedToEnvironmentSet(command.ApplicationId, command.EnvironmentSetId), command.PerformedBy);
        }
        await _documentSession.SaveChangesAsync();
    }
}