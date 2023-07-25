using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

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
        var existing = await _environmentSetRepository.GetById(command.EnvironmentSetId);
        if (existing == null)
            throw new NullReferenceException(
                "Environment Set could not be found with Id of " + command.EnvironmentSetId);

        var found = false;
        foreach (var environment in existing.DeploymentEnvironments)
            if (environment.EnvironmentSettings.Any(x => x.Key == command.VariableName))
            {
                found = true;
                break;
            }

        if (found)
        {
            return CommandResponse.FromError(Errors.DuplicateName(command.VariableName));
        }

        foreach (var environment in existing.DeploymentEnvironments)
        foreach (var setting in environment.EnvironmentSettings)
            if (setting.Key == command.VariableName)
                return CommandResponse.FromError(Errors.DuplicateName(setting.Key));

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion,
            new EnvironmentSetVariableAdded(command.VariableName));
        await _documentSession.SaveChangesAsync();
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}