using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.Applications;

public record CreateDefaultApplicationVariable(Guid ApplicationId, int ExpectedVersion, string VariableName) : IRequest<CommandResponse>;

public class CreateDefaultApplicationVariableHandler : IRequestHandler<CreateDefaultApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IApplicationRepository _querySession;
    private readonly ILogger<CreateDefaultApplicationVariableHandler> _logger;

    public CreateDefaultApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession, IApplicationRepository querySession, ILogger<CreateDefaultApplicationVariableHandler> logger)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(CreateDefaultApplicationVariable command, CancellationToken cancellationToken)
    {
        var result = new CommandResponse();
        var app = await   _querySession.GetById(command.ApplicationId);
        if (app == null)
            throw new NullReferenceException("Application could not be found");

        if (app.ApplicationDefaults.Any(x => x.Name == command.VariableName))
        {
        
            result.Errors.Add(Errors.DuplicateName(command.VariableName));
            return result;
        }
        
        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion,
            new ApplicationDefaultVariableAdded(command.VariableName));
        await _documentSession.SaveChangesAsync();
        return result;
    }
}