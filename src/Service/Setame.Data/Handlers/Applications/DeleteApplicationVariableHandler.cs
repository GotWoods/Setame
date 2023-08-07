using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record DeleteApplicationVariable(Guid ApplicationId, int ExpectedVersion, string VariableName) : IRequest<CommandResponse>;

public class DeleteApplicationVariableHandler : IRequestHandler<DeleteApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    
    private readonly ILogger<UpdateApplicationVariableHandler> _logger;

    public DeleteApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession, ILogger<UpdateApplicationVariableHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(DeleteApplicationVariable command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting variable {Variable} from application {Application}", command.VariableName, command.ApplicationId);
        var response = new CommandResponse();
        
        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationVariableDeleted(command.VariableName));
        await _documentSession.SaveChangesAsync();
        response.NewVersion = command.ExpectedVersion + 1;
        _logger.LogDebug("Variable updated");
        return response;
    }
}

