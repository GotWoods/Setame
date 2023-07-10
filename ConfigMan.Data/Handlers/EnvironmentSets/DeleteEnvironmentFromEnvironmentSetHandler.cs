using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using Marten.Internal.Sessions;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentFromEnvironmentSet(Guid EnvironmentSetId, string environmentName) : IRequest;

public class DeleteEnvironmentFromEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentFromEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;
    private readonly IUserInfo _userInfo;

    public DeleteEnvironmentFromEnvironmentSetHandler(IDocumentSession documentSession, IQuerySession querySession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _userInfo = userInfo;
    }


    public async Task Handle(DeleteEnvironmentFromEnvironmentSet command, CancellationToken cancellationToken)
    {
        var environmentRemoved = new EnvironmentRemoved(command.environmentName);
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, environmentRemoved, _userInfo.GetCurrentUserId());

        var associations = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == command.EnvironmentSetId);
        foreach (var application in associations.Applications) 
            await _documentSession.AppendToStreamAndSave<Application>(application.Id, environmentRemoved, _userInfo.GetCurrentUserId());
    }
}
