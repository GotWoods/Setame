using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using Marten.Internal.Sessions;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentFromEnvironmentSet(Guid EnvironmentSetId, string environmentName) : IRequest;

public class DeleteEnvironmentFromEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentFromEnvironmentSet>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IDocumentSessionHelper<Application> _applicationDocumentSessionHelper;
    private readonly IQuerySession _querySession;
    

    public DeleteEnvironmentFromEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IDocumentSessionHelper<Application> applicationDocumentSessionHelper, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _applicationDocumentSessionHelper = applicationDocumentSessionHelper;
        _querySession = querySession;
    }


    public async Task Handle(DeleteEnvironmentFromEnvironmentSet command, CancellationToken cancellationToken)
    {
        var environmentRemoved = new EnvironmentRemoved(command.environmentName);

        await _documentSession.AppendToStream(command.EnvironmentSetId, -1, environmentRemoved);

        var associations = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == command.EnvironmentSetId);
        foreach (var application in associations.Applications) 
            await _applicationDocumentSessionHelper.AppendToStream(application.Id, -1, environmentRemoved);

        await _documentSession.SaveChangesAsync();
        await _applicationDocumentSessionHelper.SaveChangesAsync();
    }
}
