using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record RenameApplicationVariable(Guid ApplicationId, string OldName, string NewName) : IRequest;
public class RenameApplicationVariableHandler : IRequestHandler<RenameApplicationVariable>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public RenameApplicationVariableHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(RenameApplicationVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationVariableRenamed(command.OldName, command.NewName), _userInfo.GetCurrentUserId());
    }
}