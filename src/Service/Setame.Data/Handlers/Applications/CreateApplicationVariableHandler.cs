using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.Applications;

public record CreateApplicationVariable(Guid ApplicationId, int ExpectedVersion, string VariableName) : IRequest<CommandResponse>;

public class CreateApplicationVariableHandler : IRequestHandler<CreateApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IApplicationRepository _applicationRepository;
    private readonly ILogger<CreateApplicationVariableHandler> _logger;


    public CreateApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession,
        IApplicationRepository applicationRepository, ILogger<CreateApplicationVariableHandler> logger)
    {
        _documentSession = documentSession;
        _applicationRepository = applicationRepository;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(CreateApplicationVariable command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating application variable {Variable} for {Application}", command.VariableName, command.ApplicationId);
        var result = new CommandResponse();

        var app = await _applicationRepository.GetById(command.ApplicationId);

        int envCount = 0;
        foreach (var environment in app.EnvironmentSettings)
        {
            foreach (var setting in environment.Settings)
            {
                if (setting.Name == command.VariableName)
                {
                    _logger.LogWarning("Duplicate name of {Name}", setting.Name);
                    result.Errors.Add(Errors.DuplicateName(setting.Name));
                    return result; //exit on the first error (as the variable name will exist for each environment so we don't want to see this multiple times)
                }
            }
            await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion+envCount, new ApplicationVariableAdded(environment.Name, command.VariableName));
            envCount++; //need to increment the expected version each time we append to the stream
        }
        
        await _documentSession.SaveChangesAsync();
        result.NewVersion = app.Version + envCount;
        _logger.LogDebug("Variable added");
        return result;
    }
}