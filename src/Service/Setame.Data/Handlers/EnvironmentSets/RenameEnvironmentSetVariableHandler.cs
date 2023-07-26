using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.EnvironmentSets;

public record RenameEnvironmentSetVariable(Guid EnvironmentSetId, int ExpectedVersion, string OldName, string NewName): IRequest<CommandResponse>;
public class RenameEnvironmentSetVariableHandler : IRequestHandler<RenameEnvironmentSetVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;
    private readonly ILogger<RenameEnvironmentSetVariableHandler> _logger;

    public RenameEnvironmentSetVariableHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IEnvironmentSetRepository environmentSetRepository, ILogger<RenameEnvironmentSetVariableHandler> logger)
    {
        _documentSession = documentSession;
        _environmentSetRepository = environmentSetRepository;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(RenameEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Renaming {EnvironmentSetId} variable {OldName} to {NewName}", command.EnvironmentSetId, command.OldName, command.NewName);
        var environmentSet = await _environmentSetRepository.GetById(command.EnvironmentSetId);
        foreach (var deploymentEnvironment in environmentSet.DeploymentEnvironments)
        {
            foreach (var environmentSetting in deploymentEnvironment.EnvironmentSettings)
            {
                if (environmentSetting.Key == command.NewName)
                {
                    _logger.LogWarning("Cannot rename {OldName} to {NewName} as the new name already exists", command.OldName, command.NewName);
                    return CommandResponse.FromError(Errors.DuplicateName(command.NewName));
                }
            }
        }
        
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetVariableRenamed(command.OldName, command.NewName));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("EnvironmentSet variable renamed");
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}