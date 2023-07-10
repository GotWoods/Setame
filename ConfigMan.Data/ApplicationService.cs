using ConfigMan.Data.Models;
using ConfigMan.Data.Models.Projections;
using Marten;

namespace ConfigMan.Data;

public record DeleteApplication(Guid ApplicationId, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record CreateApplication(Guid ApplicationId, string Name, string Token, Guid EnvironmentSetId, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record CreateApplicationVariable(Guid ApplicationId, string Environment, string VariableName, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record CreateDefaultApplicationVariable(Guid ApplicationId, string VariableName, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record UpdateApplicationVariable(Guid ApplicationId, string Environment, string VariableName, string NewValue, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record UpdateDefaultApplicationVariable(Guid ApplicationId, string VariableName, string NewValue, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record RenameApplicationVariable(Guid ApplicationId, string OldName, string NewName, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public interface IApplicationService
{
    Task<IList<Application>> GetAll();
    Task<Application> GetOne(Guid applicationId);
    Task Handle(DeleteApplication command);
    Task Handle(CreateApplication command);
    Task Handle(CreateApplicationVariable command);
    Task Handle(CreateDefaultApplicationVariable command);
    Task Handle(UpdateApplicationVariable command);
    Task Handle(UpdateDefaultApplicationVariable command);
    Task Handle(RenameApplicationVariable command);
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
        var allActivateApplications = _querySession.Query<ActiveApplication>().ToList();
        //var allIds = _querySession.Events.QueryAllRawEvents().Where(x => x.EventTypeName == "application_created").Select(x => x.StreamId).Distinct().ToList();
        var items = new List<Application>();
        foreach (var activeApplication in allActivateApplications)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<Application>(activeApplication.Id);
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
        _documentSession.SetHeader("user-id", command.PerformedBy);
        _documentSession.Events.StartStream<Application>(command.ApplicationId, new ApplicationCreated(command.ApplicationId, command.Name, command.Token, command.EnvironmentSetId));
        await _documentSession.SaveChangesAsync();
        // foreach (var deploymentEnvironment in environment.DeploymentEnvironments)
        // {
        //     await AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationEnvironmentAdded(deploymentEnvironment.Name), command.PerformedBy);
        //     await AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new ApplicationAssociatedToEnvironmentSet(command.ApplicationId, command.EnvironmentSetId), command.PerformedBy);
        // }
    }

    public async Task Handle(CreateApplicationVariable command)
    {
        await AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationVariableAdded(command.Environment, command.VariableName), command.PerformedBy);
    }

    public async Task Handle(CreateDefaultApplicationVariable command)
    {
        await AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationDefaultVariableAdded(command.VariableName), command.PerformedBy);
    }

    public async Task Handle(UpdateApplicationVariable command)
    {
        await AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationVariableChanged(command.Environment, command.VariableName, command.NewValue), command.PerformedBy);
    }

    public async Task Handle(UpdateDefaultApplicationVariable command)
    {
        await AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationDefaultVariableChanged(command.VariableName, command.NewValue), command.PerformedBy);
    }

    public async Task Handle(RenameApplicationVariable command)
    {
        await AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationVariableRenamed(command.OldName, command.NewName), command.PerformedBy);
    }
}