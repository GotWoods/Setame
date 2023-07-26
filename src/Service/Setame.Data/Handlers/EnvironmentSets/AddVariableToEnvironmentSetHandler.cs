using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.EnvironmentSets;

public record AddVariableToEnvironmentSet
    (Guid EnvironmentSetId, int ExpectedVersion, string VariableName) : IRequest<CommandResponse>;

public class AddVariableToEnvironmentSetHandler : IRequestHandler<AddVariableToEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;
    private readonly ILogger<AddVariableToEnvironmentSetHandler> _logger;

    public AddVariableToEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession,
        IEnvironmentSetRepository environmentSetRepository, ILogger<AddVariableToEnvironmentSetHandler> logger)
    {
        _documentSession = documentSession;
        _environmentSetRepository = environmentSetRepository;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(AddVariableToEnvironmentSet command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Adding variable {VariableName} to {EnvironmentSetId}", command.VariableName, command.EnvironmentSetId);
        var existing = await _environmentSetRepository.GetById(command.EnvironmentSetId);

        var found = false;
        foreach (var environment in existing.DeploymentEnvironments)
            if (environment.EnvironmentSettings.Any(x => x.Key == command.VariableName))
            {
                found = true;
                break;
            }

        if (found)
        {
            _logger.LogWarning("Could not add {VariableName} to {EnvironmentSetId} as the variable already exists", command.VariableName, command.EnvironmentSetId);
            return CommandResponse.FromError(Errors.DuplicateName(command.VariableName));
        }

        foreach (var environment in existing.DeploymentEnvironments)
        foreach (var setting in environment.EnvironmentSettings)
            if (setting.Key == command.VariableName)
            {
                _logger.LogWarning("Variable already exists");
                return CommandResponse.FromError(Errors.DuplicateName(setting.Key));
            }

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion,
            new EnvironmentSetVariableAdded(command.VariableName));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Add variable completed");
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}