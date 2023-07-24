﻿using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;
public record UpdateEnvironmentSetVariable(Guid EnvironmentSetId, int ExpectedVersion, string Environment, string VariableName, string VariableValue) : IRequest<CommandResponse>;

public class UpdateEnvironmentSetVariableHandler : IRequestHandler<UpdateEnvironmentSetVariable, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;   

    public UpdateEnvironmentSetVariableHandler(IDocumentSessionHelper<EnvironmentSet> documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<CommandResponse> Handle(UpdateEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetVariableChanged(command.Environment, command.VariableName, command.VariableValue));
        await _documentSession.SaveChangesAsync();
        return CommandResponse.FromSuccess(command.ExpectedVersion +1);
    }
}