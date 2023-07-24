﻿using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record CreateDefaultApplicationVariable(Guid ApplicationId, int ExpectedVersion, string VariableName) : IRequest<CommandResponse>;

public class CreateDefaultApplicationVariableHandler : IRequestHandler<CreateDefaultApplicationVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IQuerySession _querySession;

    public CreateDefaultApplicationVariableHandler(IDocumentSessionHelper<Application> documentSession, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task<CommandResponse> Handle(CreateDefaultApplicationVariable command, CancellationToken cancellationToken)
    {
        var result = new CommandResponse();
        var app = await _querySession.Events.AggregateStreamAsync<Application>(command.ApplicationId, token: cancellationToken);
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