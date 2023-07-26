using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record AddEnvironmentToEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string Name) : IRequest<CommandResponse>;
public class AddEnvironmentToEnvironmentSetHandler : IRequestHandler<AddEnvironmentToEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;
    private readonly ILogger<AddEnvironmentToEnvironmentSet> _logger;

    public AddEnvironmentToEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IEnvironmentSetRepository environmentSetRepository, ILogger<AddEnvironmentToEnvironmentSet> logger)
    {
        _documentSession = documentSession;
        _environmentSetRepository = environmentSetRepository;
        _logger = logger;
    }
    
    public async Task<CommandResponse> Handle(AddEnvironmentToEnvironmentSet command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Adding environment {Name} to {EnvironmentSetId}", command.Name, command.EnvironmentSetId);
        var existing = await _environmentSetRepository.GetById(command.EnvironmentSetId);
      

        //TODO: Add environment to all Children Applications?
        
        foreach (var deploymentEnvironment in existing.DeploymentEnvironments)
        {
            if (deploymentEnvironment.Name == command.Name)
            {
                _logger.LogWarning("Duplicate environment name of {Name}", command.Name);
                return CommandResponse.FromError(Errors.DuplicateName(command.Name));
            }
        }

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentAdded(command.Name));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Environment added");
        return CommandResponse.FromSuccess(command.ExpectedVersion+1);
    }
}