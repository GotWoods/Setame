using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record AddEnvironmentToEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string Name) : IRequest;
public class AddEnvironmentToEnvironmentSetHandler : IRequestHandler<AddEnvironmentToEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public AddEnvironmentToEnvironmentSetHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }


    public async Task Handle(AddEnvironmentToEnvironmentSet command, CancellationToken cancellationToken)
    {
        //TODO: Add environment to all Children Applications?
        //TODO: ensure no duplicates
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.ExpectedVersion, command.EnvironmentSetId, new EnvironmentAdded(command.Name), _userInfo.GetCurrentUserId());
    }
}