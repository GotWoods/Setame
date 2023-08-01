using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record CreateApplication
    (Guid ApplicationId, string Name, string Token, Guid EnvironmentSetId) : IRequest<CommandResponse>;

public class CreateApplicationHandler : IRequestHandler<CreateApplication, CommandResponse>
{
    private readonly IApplicationRepository _activeApplicationRepository;
    private readonly IDocumentSessionHelper<Application> _applicationSession;
    private readonly IDocumentSessionHelper<EnvironmentSet> _environmentSetSession;
    private readonly ILogger<CreateApplicationHandler> _logger;


    public CreateApplicationHandler(IDocumentSessionHelper<Application> applicationSession,
        IDocumentSessionHelper<EnvironmentSet> environmentSetSession,
        IApplicationRepository activeApplicationRepository, ILogger<CreateApplicationHandler> logger)
    {
        _applicationSession = applicationSession;
        _environmentSetSession = environmentSetSession;
        _activeApplicationRepository = activeApplicationRepository;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(CreateApplication command, CancellationToken cancellationToken)
    {
        var response = new CommandResponse();
        var environment = await _environmentSetSession.GetFromEventStream(command.EnvironmentSetId);
        if (environment == null)
            throw new NullReferenceException("Could not find an environment set with an id of " + command.EnvironmentSetId);

        var matchingName = _activeApplicationRepository.GetByName(command.Name);
        if (matchingName != null)
        {
            _logger.LogWarning("There is already an application named {Name}", command.Name);
            response.Errors.Add(Errors.DuplicateName(command.Name));
            return response;
        }

        var applicationEvents = new List<object> { new ApplicationCreated(command.ApplicationId, command.Name, command.Token, command.EnvironmentSetId) };
        response.NewVersion = 1;
        foreach (var deploymentEnvironment in environment.Environments)
        {
            response.NewVersion++;
            applicationEvents.Add(new ApplicationEnvironmentAdded(deploymentEnvironment.Name));
            _logger.LogDebug("Adding {Environment} to application", deploymentEnvironment.Name);
        }

        await _environmentSetSession.AppendToStream(command.EnvironmentSetId, new ApplicationAssociatedToEnvironmentSet(command.ApplicationId, command.EnvironmentSetId));
        _applicationSession.Start(command.ApplicationId, applicationEvents.ToArray());

        await _applicationSession.SaveChangesAsync();
        await _environmentSetSession.SaveChangesAsync();
        _logger.LogDebug("Application created");
        return response;
    }
}