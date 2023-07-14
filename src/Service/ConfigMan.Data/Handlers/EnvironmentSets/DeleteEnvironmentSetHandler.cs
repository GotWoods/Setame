using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentSet(Guid EnvironmentSetId) : IRequest;

public class DeleteEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public DeleteEnvironmentSetHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(DeleteEnvironmentSet command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetDeleted(command.EnvironmentSetId), _userInfo.GetCurrentUserId());
    }
}