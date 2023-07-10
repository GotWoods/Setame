using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using Marten.Internal.Sessions;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentFromEnvironmentSet(Guid EnvironmentSetId, string environmentName, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;

public class DeleteEnvironmentFromEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentFromEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;

    public DeleteEnvironmentFromEnvironmentSetHandler(IDocumentSession documentSession, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }


    public async Task Handle(DeleteEnvironmentFromEnvironmentSet command, CancellationToken cancellationToken)
    {
        var environmentRemoved = new EnvironmentRemoved(command.environmentName);
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, environmentRemoved, command.PerformedBy);

        var associations = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == command.EnvironmentSetId);
        foreach (var application in associations.Applications) 
            await _documentSession.AppendToStreamAndSave<Application>(application.Id, environmentRemoved, command.PerformedBy);
    }
}
