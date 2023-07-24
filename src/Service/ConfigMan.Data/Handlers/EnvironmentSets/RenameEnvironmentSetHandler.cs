using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string NewName) : IRequest;

public class RenameEnvironmentSetHandler : IRequestHandler<RenameEnvironmentSet>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;

    public RenameEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(RenameEnvironmentSet command, CancellationToken cancellationToken)
    {
        //var allActiveEnvironments =  _querySession.Query<ActiveEnvironmentSet>().Where(x=>x.Name == command.newName);
        //var foundWithSameName = allActiveEnvironments.Environments.Any(x => x.Value == command.newName);
        //if (foundWithSameName) throw new DuplicateNameException($"The name {command.newName} is already in use");
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetRenamed(command.EnvironmentSetId, command.NewName));
        await _documentSession.SaveChangesAsync();
    }
}