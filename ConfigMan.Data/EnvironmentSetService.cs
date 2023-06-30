using System.Data;
using ConfigMan.Data.Models;
using ConfigMan.Data.Models.Projections;
using JasperFx.Core;
using Marten;

public record CreateEnvironmentSet(string Name, Guid PerformedBy);

public record RenameEnvironmentSet(Guid EnvironmentSetId, string newName, Guid PerformedBy);

public record DeleteEnvironmentSet(Guid EnvironmentSetId, Guid PerformedBy);

public record AddEnvironmentToEnvironmentSet(Guid EnvironmentSetId, string Name, Guid PerformedBy);

public record RenameEnvironment(Guid EnvironmentSetId, string newName, Guid PerformedBy);

public record DeleteEnvironmentFromEnvironmentSet(Guid EnvironmentSetId, string environmentName, Guid PerformedBy);

public record AddVariableToEnvironmentSet(Guid EnvironmentSetId, string VariableName, Guid PerformedBy);

public record UpdateEnvironmentSetVariable(Guid EnvironmentSetId, string Environment, string VariableName, string VariableValue, Guid PerformedBy);

public record RenameEnvironmentSetVariable(Guid EnvironmentSetId, string OldName, string NewName, Guid PerformedBy);

public interface IEnvironmentSetService
{
    Task<List<EnvironmentSet>> GetAll();
    Task Handle(CreateEnvironmentSet command);
    Task Handle(RenameEnvironmentSet command);
    Task Handle(DeleteEnvironmentSet command);
    Task Handle(AddEnvironmentToEnvironmentSet command);
    Task Handle(DeleteEnvironmentFromEnvironmentSet command);
    Task Handle(AddVariableToEnvironmentSet command);
    Task Handle(UpdateEnvironmentSetVariable command);
    Task Handle(RenameEnvironmentSetVariable command);
}

public class EnvironmentSetService : IEnvironmentSetService
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;

    public EnvironmentSetService(IDocumentSession documentSession, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task<List<EnvironmentSet>> GetAll()
    {
        var summary = await _querySession.Query<EnvironmentSetSummary>().FirstOrDefaultAsync();
        var items = new List<EnvironmentSet>();

        if (summary == null)
            return items;

        foreach (var id in summary.Environments.Keys)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(id);
            if (aggregateStreamAsync != null)
                items.Add(aggregateStreamAsync);
        }

        var sorted = items.OrderBy(x => x.Name);
        return sorted.ToList();
    }

//
    public async Task Handle(CreateEnvironmentSet command)
    {
        if (string.IsNullOrEmpty(command.Name)) throw new ArgumentNullException(nameof(command.Name));

        var summaryDocument = _querySession.Query<EnvironmentSetSummary>().FirstOrDefault();
        if (summaryDocument != null)
        {
            var foundWithSameName = summaryDocument.Environments.Any(x => x.Value == command.Name);
            if (foundWithSameName) throw new DuplicateNameException($"The name {command.Name} is already in use");
        }

        var id = CombGuidIdGeneration.NewGuid();
        _documentSession.SetHeader("user-id", command.PerformedBy);
        _documentSession.Events.StartStream<EnvironmentSet>(id, new EnvironmentSetCreated(id, command.Name));
        await _documentSession.SaveChangesAsync();
    }

    public async Task Handle(RenameEnvironmentSet command)
    {
        var summaryDocument = _querySession.Query<EnvironmentSetSummary>().FirstOrDefault();
        if (summaryDocument != null)
        {
            var foundWithSameName = summaryDocument.Environments.Any(x => x.Value == command.newName);
            if (foundWithSameName) throw new DuplicateNameException($"The name {command.newName} is already in use");
        }


        _documentSession.SetHeader("user-id", command.PerformedBy);
        await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentSetRenamed(command.EnvironmentSetId, command.newName)));
        await _documentSession.SaveChangesAsync();
    }

    public async Task Handle(DeleteEnvironmentSet command)
    {
        //TODO: query all applications and see if any are using this environmentSet
        //   throw new InvalidOperationException("Can not delete an Environment Set when an application is associated to the environment set");
        _documentSession.SetHeader("user-id", command.PerformedBy);
        await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentSetDeleted(command.EnvironmentSetId)));
        await _documentSession.SaveChangesAsync();
    }

    public async Task Handle(AddEnvironmentToEnvironmentSet command)
    {
        //TODO: Add environment to all Children Applications. What should the values be for these items?
        _documentSession.SetHeader("user-id", command.PerformedBy);
        await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentAdded(command.Name)));
        await _documentSession.SaveChangesAsync();
    }

    public async Task Handle(DeleteEnvironmentFromEnvironmentSet command)
    {
        //TODO: remove from all children applications?
        _documentSession.SetHeader("user-id", command.PerformedBy);
        await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentRemoved(command.environmentName)));
        await _documentSession.SaveChangesAsync();
        throw new NotImplementedException();
    }

    public async Task Handle(AddVariableToEnvironmentSet command)
    {
        //TODO: ensure the variable is not already created
        _documentSession.SetHeader("user-id", command.PerformedBy);
        await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentSetVariableAdded(command.VariableName)));
        await _documentSession.SaveChangesAsync();
    }

    public async Task Handle(UpdateEnvironmentSetVariable command)
    {
        _documentSession.SetHeader("user-id", command.PerformedBy);
        await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentSetVariableChanged(command.Environment, command.VariableName, command.VariableValue)));
        await _documentSession.SaveChangesAsync();
    }

    public async Task Handle(RenameEnvironmentSetVariable command)
    {
        //TODO: ensure the variable name does not collide
        _documentSession.SetHeader("user-id", command.PerformedBy);
        await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentSetVariableRenamed(command.OldName, command.NewName)));
        await _documentSession.SaveChangesAsync();
    }


    // public async Task Handle(RenameEnvironment command)
    // {
    //     //TODO: Add environment to all Children Applications. What should the values be for these items?
    //     _documentSession.SetHeader("user-id", command.PerformedBy);
    //     await _documentSession.Events.WriteToAggregate<EnvironmentSet>(command.EnvironmentSetId, stream => stream.AppendOne(new EnvironmentRenamed(command.Name)));
    //     await _documentSession.SaveChangesAsync();
    // }
}