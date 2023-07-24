using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record CreateApplication(Guid ApplicationId, string Name, string Token, Guid EnvironmentSetId) : IRequest<CommandResponse>;



public class CreateApplicationHandler : IRequestHandler<CreateApplication, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _applicationSession;
    private readonly IDocumentSessionHelper<EnvironmentSet> _environmentSetSession;
    private readonly IApplicationRepository _activeApplicationRepository;


    public CreateApplicationHandler(IDocumentSessionHelper<Application> applicationSession, IDocumentSessionHelper<EnvironmentSet> environmentSetSession, IApplicationRepository activeApplicationRepository)
    {
        _applicationSession = applicationSession;
        _environmentSetSession = environmentSetSession;
        _activeApplicationRepository = activeApplicationRepository;
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
            response.Errors.Add(Errors.DuplicateName(command.Name));
            return response;
        }


        var applicationEvents = new List<object> { new ApplicationCreated(command.ApplicationId, command.Name, command.Token, command.EnvironmentSetId) };
        response.NewVersion = 1;
        foreach (var deploymentEnvironment in environment.DeploymentEnvironments)
        {
            response.NewVersion++;
            applicationEvents.Add(new ApplicationEnvironmentAdded(deploymentEnvironment.Name));
            await _environmentSetSession.AppendToStream(command.EnvironmentSetId, new ApplicationAssociatedToEnvironmentSet(command.ApplicationId, command.EnvironmentSetId));
        }

        _applicationSession.Start(command.ApplicationId, applicationEvents.ToArray());

        await _applicationSession.SaveChangesAsync();
        await _environmentSetSession.SaveChangesAsync();

        return response;
    }
}