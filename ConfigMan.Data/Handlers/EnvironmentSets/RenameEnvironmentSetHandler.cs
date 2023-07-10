using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record AddEnvironmentToEnvironmentSet(Guid EnvironmentSetId, string Name, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;

public record RenameEnvironment(Guid EnvironmentSetId, string OldName, string NewName, Guid PerformedBy) : ApplicationCommand(PerformedBy);

public record DeleteEnvironmentFromEnvironmentSet(Guid EnvironmentSetId, string environmentName, Guid PerformedBy) : ApplicationCommand(PerformedBy);


public record RenameEnvironmentSet(Guid EnvironmentSetId, string newName, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;


public class AddEnvironmentToEnvironmentSetHandler : IRequestHandler<AddEnvironmentToEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;

    public AddEnvironmentToEnvironmentSetHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }


    public Task Handle(AddEnvironmentToEnvironmentSet request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

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


