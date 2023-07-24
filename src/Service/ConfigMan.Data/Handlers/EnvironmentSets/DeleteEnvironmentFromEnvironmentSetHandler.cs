using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentFromEnvironmentSet
    (Guid EnvironmentSetId, int ExpectedVersion, string EnvironmentName) : IRequest<CommandResponse>;

public class
    DeleteEnvironmentFromEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentFromEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _applicationDocumentSessionHelper;
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IQuerySession _querySession;


    public DeleteEnvironmentFromEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession,
        IDocumentSessionHelper<Application> applicationDocumentSessionHelper, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _applicationDocumentSessionHelper = applicationDocumentSessionHelper;
        _querySession = querySession;
    }


    public async Task<CommandResponse> Handle(DeleteEnvironmentFromEnvironmentSet command, CancellationToken cancellationToken)
    {
        var environmentRemoved = new EnvironmentRemoved(command.EnvironmentName);

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, environmentRemoved);

        var associations = _querySession.Query<EnvironmentSetApplicationAssociation>()
            .First(x => x.Id == command.EnvironmentSetId);
        foreach (var application in associations.Applications)
            await _applicationDocumentSessionHelper.AppendToStream(application.Id, -1, environmentRemoved);

        await _documentSession.SaveChangesAsync();
        await _applicationDocumentSessionHelper.SaveChangesAsync();
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}