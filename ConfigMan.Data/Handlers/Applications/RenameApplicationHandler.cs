using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record RenameApplication(Guid ApplicationId, int ExpectedVersion, string NewName) : IRequest;
public class RenameApplicationHandler : IRequestHandler<RenameApplication>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public RenameApplicationHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(RenameApplication command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<Application>(command.ExpectedVersion, command.ApplicationId, new ApplicationRenamed(command.NewName), _userInfo.GetCurrentUserId());
    }
}