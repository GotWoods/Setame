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
        var existing = await _environmentSetRepository.GetById(command.EnvironmentSetId);
        if (existing == null)
            throw new NullReferenceException("Environment Set could not be found with Id of " + command.EnvironmentSetId);

        //TODO: Add environment to all Children Applications?
        
        foreach (var deploymentEnvironment in existing.DeploymentEnvironments)
        {
            if (deploymentEnvironment.Name == command.Name)
            {
                return CommandResponse.FromError(Errors.DuplicateName(command.Name));
            }
        }

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentAdded(command.Name));
        await _documentSession.SaveChangesAsync();
        return CommandResponse.FromSuccess(command.ExpectedVersion+1);
    }
}