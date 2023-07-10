using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record AddVariableToEnvironmentSet(Guid EnvironmentSetId, string VariableName, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;
public class AddVariableToEnvironmentSetHandler : IRequestHandler<AddVariableToEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;

    public AddVariableToEnvironmentSetHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(AddVariableToEnvironmentSet command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetVariableAdded(command.VariableName), command.PerformedBy);
    }
}