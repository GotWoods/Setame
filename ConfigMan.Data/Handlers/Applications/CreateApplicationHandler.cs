using ConfigMan.Data.Models;
using Marten;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Handlers.Applications;

public record CreateApplication(Guid ApplicationId, string Name, string Token, Guid EnvironmentSetId) : IRequest;

public class CreateApplicationHandler : IRequestHandler<CreateApplication>
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;
    private readonly IUserInfo _userInfo;

    public CreateApplicationHandler(IDocumentSession documentSession, IQuerySession querySession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _userInfo = userInfo;
    }

    public async Task Handle(CreateApplication command, CancellationToken cancellationToken)
    {
        var environment = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(command.EnvironmentSetId, token: cancellationToken);

        _documentSession.Events.StartStream<Application>(command.ApplicationId, new ApplicationCreated(command.ApplicationId, command.Name, command.Token, command.EnvironmentSetId));
        await _documentSession.SaveChangesAsync(cancellationToken);

        foreach (var deploymentEnvironment in environment.DeploymentEnvironments)
        {
            await _documentSession.AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationEnvironmentAdded(deploymentEnvironment.Name), _userInfo.GetCurrentUserId());
            await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new ApplicationAssociatedToEnvironmentSet(command.ApplicationId, command.EnvironmentSetId), _userInfo.GetCurrentUserId());
        }
    }
}