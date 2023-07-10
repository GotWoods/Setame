using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironmentSetVariable(Guid EnvironmentSetId, string OldName, string NewName): IRequest;
public class RenameEnvironmentSetVariableHandler : IRequestHandler<RenameEnvironmentSetVariable>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public RenameEnvironmentSetVariableHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(RenameEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetVariableRenamed(command.OldName, command.NewName), _userInfo.GetCurrentUserId());
    }
}