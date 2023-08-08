using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.EnvironmentSets;

public record AddEnvironmentToEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string Name) : IRequest<CommandResponse>;
public class AddEnvironmentToEnvironmentSetHandler : IRequestHandler<AddEnvironmentToEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _environmentSetDocumentSession;
    private readonly IDocumentSessionHelper<Application> _applicationDocumentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;
    private readonly IEnvironmentSetApplicationAssociationRepository _environmentSetApplicationAssociationRepository;
    private readonly ILogger<AddEnvironmentToEnvironmentSet> _logger;

    public AddEnvironmentToEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> environmentSetDocumentSession, IDocumentSessionHelper<Application> applicationDocumentSession, IEnvironmentSetRepository environmentSetRepository, IEnvironmentSetApplicationAssociationRepository environmentSetApplicationAssociationRepository,  ILogger<AddEnvironmentToEnvironmentSet> logger)
    {
        _environmentSetDocumentSession = environmentSetDocumentSession;
        _applicationDocumentSession = applicationDocumentSession;
        _environmentSetRepository = environmentSetRepository;
        _environmentSetApplicationAssociationRepository = environmentSetApplicationAssociationRepository;
        _logger = logger;
    }
    
    public async Task<CommandResponse> Handle(AddEnvironmentToEnvironmentSet command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Adding environment {Name} to {EnvironmentSetId}", command.Name, command.EnvironmentSetId);
        var existing = await _environmentSetRepository.GetById(command.EnvironmentSetId);
      

     
        
        foreach (var deploymentEnvironment in existing.Environments)
        {
            if (deploymentEnvironment.Name == command.Name)
            {
                _logger.LogWarning("Duplicate environment name of {Name}", command.Name);
                return CommandResponse.FromError(Errors.DuplicateName(command.Name));
            }
        }

        var association = _environmentSetApplicationAssociationRepository.Get(command.EnvironmentSetId);
        foreach (var application in association.Applications)
        {
            await _applicationDocumentSession.AppendToStream(application.Id, new ApplicationEnvironmentAdded(command.Name));
        }


        await _environmentSetDocumentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentAdded(command.Name));
        await _environmentSetDocumentSession.SaveChangesAsync();
        await _applicationDocumentSession.SaveChangesAsync();
        _logger.LogDebug("Environment added");
        return CommandResponse.FromSuccess(command.ExpectedVersion+1);
    }
}