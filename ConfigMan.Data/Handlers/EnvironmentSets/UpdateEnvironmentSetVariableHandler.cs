using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;
public record UpdateEnvironmentSetVariable(Guid EnvironmentSetId, string Environment, string VariableName, string VariableValue, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;

public class UpdateEnvironmentSetVariableHandler : IRequestHandler<UpdateEnvironmentSetVariable>
{
    private readonly IDocumentSession _documentSession;

    public UpdateEnvironmentSetVariableHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(UpdateEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetVariableChanged(command.Environment, command.VariableName, command.VariableValue), command.PerformedBy);
    }
}