﻿using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record UpdateApplicationVariable(Guid ApplicationId, int ExpectedVersion, string Environment,
    string VariableName, string NewValue) : IRequest<CommandResponse>;

public class UpdateApplicationVariableHandler : IRequestHandler<UpdateApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IQuerySession _querySession;

    public UpdateApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task<CommandResponse> Handle(UpdateApplicationVariable command, CancellationToken cancellationToken)
    {
        var response = new CommandResponse();

        var existing = await _querySession.Events.AggregateStreamAsync<Application>(command.ApplicationId, token: cancellationToken);
        if (existing == null)
        {
            response.Errors.Add(Errors.ApplicationNotFound(command.ApplicationId));
            return response;
        }

        var environmentSetting = existing.EnvironmentSettings.FirstOrDefault(x=>x.Name == command.Environment);
        if (environmentSetting == null)
        {
            response.Errors.Add(Errors.EnvironmentNotFound(command.Environment));
            return response;
        }

        if (environmentSetting.Settings.FirstOrDefault(x => x.Name == command.VariableName) == null)
        {
            response.Errors.Add(Errors.VariableNotFound(command.VariableName));
            return response;
        }
       

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationVariableChanged(command.Environment, command.VariableName, command.NewValue));
        await _documentSession.SaveChangesAsync();
        response.NewVersion = command.ExpectedVersion + 1;
        return response;
    }
}