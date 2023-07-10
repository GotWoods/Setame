using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;








public record RenameEnvironmentSet(Guid EnvironmentSetId, string newName, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;

public class RenameEnvironmentSetHandler : IRequestHandler<RenameEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;

    public RenameEnvironmentSetHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(RenameEnvironmentSet command, CancellationToken cancellationToken)
    {
        //var allActiveEnvironments =  _querySession.Query<ActiveEnvironmentSet>().Where(x=>x.Name == command.newName);
        //var foundWithSameName = allActiveEnvironments.Environments.Any(x => x.Value == command.newName);
        //if (foundWithSameName) throw new DuplicateNameException($"The name {command.newName} is already in use");
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetRenamed(command.EnvironmentSetId, command.newName), command.PerformedBy);
    }
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