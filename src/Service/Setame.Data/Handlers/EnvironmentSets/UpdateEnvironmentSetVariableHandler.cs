using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.EnvironmentSets;

public record UpdateEnvironmentSetVariable(Guid EnvironmentSetId, int ExpectedVersion, string Environment, string VariableName, string VariableValue) : IRequest<CommandResponse>;

public class UpdateEnvironmentSetVariableHandler : IRequestHandler<UpdateEnvironmentSetVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;
    private readonly ILogger<UpdateEnvironmentSetVariableHandler> _logger;

    public UpdateEnvironmentSetVariableHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IEnvironmentSetRepository environmentSetRepository, ILogger<UpdateEnvironmentSetVariableHandler> logger)
    {
        _documentSession = documentSession;
        _environmentSetRepository = environmentSetRepository;
        _logger = logger;
    }


    public async Task<CommandResponse> Handle(UpdateEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating variable {VariableName} in environment {Environment} in environment set {EnvironmentSetId}", command.VariableName, command.Environment, command.EnvironmentSetId);

        var existingEnvironmentSet = await _environmentSetRepository.GetById(command.EnvironmentSetId);
        var firstEnvironment = existingEnvironmentSet.Environments.FirstOrDefault();
        if (firstEnvironment == null)
            throw new NullReferenceException("An environment set was found but it had no environments");
        if (!firstEnvironment.Settings.ContainsKey(command.VariableName))
            throw new NullReferenceException("The variable name was not found so no update could be done");
        
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetVariableChanged(command.Environment, command.VariableName, command.VariableValue));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Updated variable");
        return CommandResponse.FromSuccess(command.ExpectedVersion + 1);
    }
}