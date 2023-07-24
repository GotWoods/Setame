using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironmentSetVariable(Guid EnvironmentSetId, int ExpectedVersion, string OldName, string NewName): IRequest;
public class RenameEnvironmentSetVariableHandler : IRequestHandler<RenameEnvironmentSetVariable>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;

    public RenameEnvironmentSetVariableHandler(IDocumentSessionHelper<EnvironmentSet> documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(RenameEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetVariableRenamed(command.OldName, command.NewName));
    }
}