using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;

public record AddVariableToEnvironmentSet(Guid EnvironmentSetId, string VariableName, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record UpdateEnvironmentSetVariable(Guid EnvironmentSetId, string Environment, string VariableName, string VariableValue, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record RenameEnvironmentSetVariable(Guid EnvironmentSetId, string OldName, string NewName, Guid PerformedBy) : ApplicationCommand(PerformedBy);


public interface IEnvironmentSetService
{
    Task<List<EnvironmentSet>> GetAll();
    Task Handle(AddVariableToEnvironmentSet command);
    Task Handle(UpdateEnvironmentSetVariable command);
    Task Handle(RenameEnvironmentSetVariable command);
    Task<EnvironmentSet> GetOne(Guid environmentSetId);
}

public class EnvironmentSetService : ServiceBase, IEnvironmentSetService
{
    private readonly IQuerySession _querySession;

    public EnvironmentSetService(IDocumentSession documentSession, IQuerySession querySession) : base(documentSession)
    {
        _querySession = querySession;
    }

    public async Task<List<EnvironmentSet>> GetAll()
    {
        var summary = await _querySession.Query<ActiveEnvironmentSet>().ToListAsync();
        var items = new List<EnvironmentSet>();

        foreach (var activeEnvironmentSet in summary)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(activeEnvironmentSet.Id);
            if (aggregateStreamAsync != null)
                items.Add(aggregateStreamAsync);
        }

        var sorted = items.OrderBy(x => x.Name);
        return sorted.ToList();
    }

    public async Task<EnvironmentSet> GetOne(Guid environmentSetId)
    {
        return await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(environmentSetId) ?? throw new NullReferenceException("The environment set could not be found");
    }





    public async Task Handle(AddVariableToEnvironmentSet command)
    {
        //TODO: ensure the variable is not already created
        await AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetVariableAdded(command.VariableName), command.PerformedBy);
    }

    public async Task Handle(UpdateEnvironmentSetVariable command)
    {
        //TODO: basic input validation
        await AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetVariableChanged(command.Environment, command.VariableName, command.VariableValue), command.PerformedBy);
    }

    public async Task Handle(RenameEnvironmentSetVariable command)
    {
        //TODO: ensure the variable name does not collide
        await AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetVariableRenamed(command.OldName, command.NewName), command.PerformedBy);
    }


    // public async Task Handle(RenameEnvironment command)
    // {
    //     //TODO: Add environment to all Children Applications. What should the values be for these items?
    //     _documentSession.SetHeader("user-id", command.PerformedBy);
    //     await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentRenamed(command.Name)));
    //     await _documentSession.SaveChangesAsync();
    // }
}