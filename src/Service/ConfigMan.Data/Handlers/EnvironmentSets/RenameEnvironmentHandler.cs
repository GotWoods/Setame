using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;
using Marten.Internal.Sessions;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironment(Guid EnvironmentSetId, int ExpectedVersion, string OldName, string NewName) : IRequest<CommandResponse>;
public class RenameEnvironmentHandler : IRequestHandler<RenameEnvironment, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IDocumentSessionHelper<Application> _applicationSession;
    private readonly IQuerySession _querySession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;

    public RenameEnvironmentHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IDocumentSessionHelper<Application> applicationSession,  IQuerySession querySession, IEnvironmentSetRepository environmentSetRepository)
    {
        _documentSession = documentSession;
        _applicationSession = applicationSession;
        _querySession = querySession;
        _environmentSetRepository = environmentSetRepository;
    }


    public async Task<CommandResponse> Handle(RenameEnvironment command, CancellationToken cancellationToken)
    {
        var environmentSet = await _environmentSetRepository.GetById(command.EnvironmentSetId);
        if (environmentSet == null)
            throw new NullReferenceException("Environment Set could not be found");

        if (environmentSet.DeploymentEnvironments.Any(x => x.Name == command.NewName))
            return CommandResponse.FromError(Errors.DuplicateName(command.NewName));

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentRenamed(command.OldName, command.NewName));

        var associations = _querySession.Query<EnvironmentSetApplicationAssociation>().First(x => x.Id == command.EnvironmentSetId);
        foreach (var application in associations.Applications) 
            await _applicationSession.AppendToStream(application.Id, -1, new EnvironmentRenamed(command.OldName, command.NewName));

        await _documentSession.SaveChangesAsync();
        
        return CommandResponse.FromSuccess(command.ExpectedVersion +1);
    }
}