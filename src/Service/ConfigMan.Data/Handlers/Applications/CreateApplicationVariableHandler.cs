using ConfigMan.Data.Models;
using Marten;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Handlers.Applications;

public record CreateApplicationVariable(Guid ApplicationId, int ExpectedVersion, string Environment, string VariableName) : IRequest<CommandResponse>;

public class CreateApplicationVariableHandler : IRequestHandler<CreateApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IQuerySession _querySession;

    public CreateApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task<CommandResponse> Handle(CreateApplicationVariable command, CancellationToken cancellationToken)
    {
        var result = new CommandResponse();

        var app = await _querySession.Events.AggregateStreamAsync<Application>(command.ApplicationId, token: cancellationToken);
        if (app == null)
            throw new NullReferenceException("Application could not be found");

        if (app.EnvironmentSettings.FirstOrDefault(x => x.Name == command.Environment) == null)
            throw new NullReferenceException("Environment " + command.Environment + " could not be found on the application");

        foreach (var environment in app.EnvironmentSettings)
        {
            foreach (var setting in environment.Settings)
            {
                if (setting.Name == command.VariableName)
                {
                    result.Errors.Add(Errors.DuplicateName(setting.Name));
                    return result; //exit on the first error (as the variable name will exist for each environment so we don't want to see this multiple times)
                }
            }
        }

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationVariableAdded(command.Environment, command.VariableName));
        await _documentSession.SaveChangesAsync();
        result.NewVersion = app.Version + 1;
        return result;
    }
}

