using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record RenameApplication(Guid ApplicationId, int ExpectedVersion, string NewName) : IRequest;
public class RenameApplicationHandler : IRequestHandler<RenameApplication>
{
    private readonly IDocumentSession _documentSession;
    private readonly IQuerySession _querySession;
    private readonly IUserInfo _userInfo;

    public RenameApplicationHandler(IDocumentSession documentSession, IQuerySession querySession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _querySession = querySession;
        _userInfo = userInfo;
    }

    public async Task Handle(RenameApplication command, CancellationToken cancellationToken)
    {
        var existing = _querySession.Query<Application>().FirstOrDefault(x => x.Name == command.NewName);
        if (existing != null)
            throw new ApplicationException("Conflict");

        await _documentSession.AppendToStreamAndSave<Application>(command.ExpectedVersion, command.ApplicationId, new ApplicationRenamed(command.NewName), _userInfo.GetCurrentUserId());
    }
}