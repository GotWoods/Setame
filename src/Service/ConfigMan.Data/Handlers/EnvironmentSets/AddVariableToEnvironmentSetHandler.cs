using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record AddVariableToEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string VariableName) : IRequest;
public class AddVariableToEnvironmentSetHandler : IRequestHandler<AddVariableToEnvironmentSet>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;

    public AddVariableToEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(AddVariableToEnvironmentSet command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetVariableAdded(command.VariableName));
    }
}