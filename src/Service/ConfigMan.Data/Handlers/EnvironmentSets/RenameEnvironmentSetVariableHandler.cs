using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

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
        var environmentSet = await _environmentSetRepository.GetById(command.EnvironmentSetId);
        if (environmentSet == null)
            throw new NullReferenceException("Environment Set could not be found");

        foreach (var deploymentEnvironment in environmentSet.DeploymentEnvironments)
        {
            foreach (var environmentSetting in deploymentEnvironment.EnvironmentSettings)
            {
                if (environmentSetting.Key == command.NewName)
                    return CommandResponse.FromError(Errors.DuplicateName(command.NewName));
            }
        }
        
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetVariableRenamed(command.OldName, command.NewName));
        await _documentSession.SaveChangesAsync();
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}