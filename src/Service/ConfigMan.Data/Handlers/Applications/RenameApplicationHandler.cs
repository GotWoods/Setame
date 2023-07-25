using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.Applications;

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
        var response = new CommandResponse();
        var existing =  await _querySession.GetById(command.ApplicationId);
        if (existing == null)
           response.Errors.Add(Errors.ApplicationNotFound(command.ApplicationId));

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationRenamed(command.NewName));
        await _documentSession.SaveChangesAsync();
        return response;
    }
}