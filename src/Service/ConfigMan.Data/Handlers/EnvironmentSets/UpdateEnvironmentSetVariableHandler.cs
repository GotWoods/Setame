using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;
public record UpdateEnvironmentSetVariable(Guid EnvironmentSetId, int ExpectedVersion, string Environment, string VariableName, string VariableValue) : IRequest;

public class UpdateEnvironmentSetVariableHandler : IRequestHandler<UpdateEnvironmentSetVariable>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;   

    public UpdateEnvironmentSetVariableHandler(IDocumentSessionHelper<EnvironmentSet> documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(UpdateEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetVariableChanged(command.Environment, command.VariableName, command.VariableValue));
    }
}