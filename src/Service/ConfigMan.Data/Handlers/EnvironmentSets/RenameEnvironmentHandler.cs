using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using Marten.Internal.Sessions;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironment(Guid EnvironmentSetId, int ExpectedVersion, string OldName, string NewName) : IRequest;
public class RenameEnvironmentHandler : IRequestHandler<RenameEnvironment>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IDocumentSessionHelper<Application> _applicationSession;
    private readonly IQuerySession _querySession;

    public RenameEnvironmentHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IDocumentSessionHelper<Application> applicationSession,  IQuerySession querySession)
    {
        _documentSession = documentSession;
        _applicationSession = applicationSession;
        _querySession = querySession;
    }


    public async Task Handle(RenameEnvironment command, CancellationToken cancellationToken)
    {
        var environmentRenamed = new EnvironmentRenamed(command.OldName, command.NewName);
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, environmentRenamed);

        var associations = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == command.EnvironmentSetId);
        foreach (var application in associations.Applications) 
            await _applicationSession.AppendToStream(application.Id, -1, environmentRenamed);
    }
}