using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record RenameApplication(Guid ApplicationId, int ExpectedVersion, string NewName) : IRequest<CommandResponse>;
public class RenameApplicationHandler : IRequestHandler<RenameApplication, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IApplicationRepository _querySession;
    private readonly ILogger<RenameApplicationHandler> _logger;

    public RenameApplicationHandler(IDocumentSessionHelper<Application> documentSession, IApplicationRepository querySession, ILogger<RenameApplicationHandler> logger)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(RenameApplication command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Renaming {Application} to {NewName}", command.ApplicationId, command.NewName);
        var response = new CommandResponse();
        var existing =  _querySession.GetByName(command.NewName);
        if (existing == null)
        {
            _logger.LogWarning("Could not rename to {NewName} as an application already has that name", command.NewName);
            response.Errors.Add(Errors.ApplicationNotFound(command.ApplicationId));
        }

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationRenamed(command.NewName));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Application renamed");
        return response;
    }
}