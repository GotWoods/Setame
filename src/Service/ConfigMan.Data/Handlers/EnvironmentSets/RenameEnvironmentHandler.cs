using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironment
    (Guid EnvironmentSetId, int ExpectedVersion, string OldName, string NewName) : IRequest<CommandResponse>;

public class RenameEnvironmentHandler : IRequestHandler<RenameEnvironment, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _applicationSession;
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetApplicationAssociationRepository _environmentSetApplicationAssociationRepository;
    private readonly ILogger<RenameEnvironmentHandler> _logger;
    private readonly IEnvironmentSetRepository _environmentSetRepository;

    public RenameEnvironmentHandler(IDocumentSessionHelper<EnvironmentSet> documentSession,
        IDocumentSessionHelper<Application> applicationSession, IEnvironmentSetRepository environmentSetRepository,
        IEnvironmentSetApplicationAssociationRepository environmentSetApplicationAssociationRepository, ILogger<RenameEnvironmentHandler> logger)
    {
        _documentSession = documentSession;
        _applicationSession = applicationSession;
        _environmentSetRepository = environmentSetRepository;
        _environmentSetApplicationAssociationRepository = environmentSetApplicationAssociationRepository;
        _logger = logger;
    }


    public async Task<CommandResponse> Handle(RenameEnvironment command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Renaming {EnvironmentSet} to {NewName}", command.EnvironmentSetId, command.NewName);
        var environmentSet = await _environmentSetRepository.GetById(command.EnvironmentSetId);

        if (environmentSet.DeploymentEnvironments.Any(x => x.Name == command.NewName))
        {
            _logger.LogWarning("Could not rename {EnvironmentSet} to {NewName} as the environment already exists", command.EnvironmentSetId, command.NewName);
            return CommandResponse.FromError(Errors.DuplicateName(command.NewName));
        }

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentRenamed(command.OldName, command.NewName));

        var associations = _environmentSetApplicationAssociationRepository.Get(command.EnvironmentSetId);
        foreach (var application in associations.Applications)
        {
            _logger.LogDebug("Renaming {EnvironmentSet} to {NewName} for {Application}", command.EnvironmentSetId, command.NewName, application.Id);
            await _applicationSession.AppendToStream(application.Id, new EnvironmentRenamed(command.OldName, command.NewName));
        }

        await _documentSession.SaveChangesAsync();
        await _applicationSession.SaveChangesAsync();
        _logger.LogDebug("Rename environment completed");
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}