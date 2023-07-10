using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironmentSetVariable(Guid EnvironmentSetId, string OldName, string NewName, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;
public class RenameEnvironmentSetVariableHandler : IRequestHandler<RenameEnvironmentSetVariable>
{
    private readonly IDocumentSession _documentSession;

    public RenameEnvironmentSetVariableHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(RenameEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetVariableRenamed(command.OldName, command.NewName), command.PerformedBy);
    }
}