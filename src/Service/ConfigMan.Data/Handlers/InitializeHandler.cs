using System.Security;
using ConfigMan.Data.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers;

public record InitializeApplication(string AdminEmailAddress, string Password) : IRequest;

public class InitializeHandler : IRequestHandler<InitializeApplication>
{
    private readonly IDocumentSession _documentSession;
    private readonly ILogger<InitializeApplication> _logger;
    private readonly IQuerySession _querySession;
    private readonly IUserService _userService;

    public InitializeHandler(IUserService userService, IDocumentSession documentSession, IQuerySession querySession,
        ILogger<InitializeApplication> logger)
    {
        _userService = userService;
        _documentSession = documentSession;
        _querySession = querySession;
        _logger = logger;
    }

    public async Task Handle(InitializeApplication request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Application is being initialized");
        var currentStatus =
            await _querySession.Events.AggregateStreamAsync<ServiceStatus>(ServiceStatus.ServiceId,
                token: cancellationToken);
        if (CheckIfInitialized(currentStatus)) throw new SecurityException("Application can not be initialized twice");

        var newAdminUser = new User
        {
            Username = request.AdminEmailAddress,
            Id = Guid.NewGuid()
        };
        await _userService.CreateUserAsync(newAdminUser, request.Password);

        _documentSession.Events.StartStream<ServiceStatus>(ServiceStatus.ServiceId, new ApplicationInitialized());
        await _documentSession.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Application is initialized. New administrative account is now {Email}",
            request.AdminEmailAddress);
    }

    private bool CheckIfInitialized(ServiceStatus? currentStatus)
    {
        if (currentStatus == null)
        {
            _logger.LogDebug("There is no current service status entry");
            return false;
        }

        if (currentStatus.IsInitialized)
        {
            _logger.LogDebug("service status entry shows the application as initialized");
            return true;
        }

        _logger.LogDebug("service status entry exists but application is not set to Initialized yet");
        return false;
    }
}