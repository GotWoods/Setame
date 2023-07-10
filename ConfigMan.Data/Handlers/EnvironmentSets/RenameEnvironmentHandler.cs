using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using Marten.Internal.Sessions;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironment(Guid EnvironmentSetId, string OldName, string NewName) : IRequest;
public class RenameEnvironmentHandler : IRequestHandler<RenameEnvironment>
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;
    private readonly IUserInfo _userInfo;

    public RenameEnvironmentHandler(IDocumentSession documentSession, IQuerySession querySession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _userInfo = userInfo;
    }


    public async Task Handle(RenameEnvironment command, CancellationToken cancellationToken)
    {
        var environmentRenamed = new EnvironmentRenamed(command.OldName, command.NewName);
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, environmentRenamed, _userInfo.GetCurrentUserId());

        var associations = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == command.EnvironmentSetId);
        foreach (var application in associations.Applications) await _documentSession.AppendToStreamAndSave<Application>(application.Id, environmentRenamed, _userInfo.GetCurrentUserId());
    }
}