using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record AddVariableToEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string VariableName) : IRequest;
public class AddVariableToEnvironmentSetHandler : IRequestHandler<AddVariableToEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public AddVariableToEnvironmentSetHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(AddVariableToEnvironmentSet command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.ExpectedVersion, command.EnvironmentSetId, new EnvironmentSetVariableAdded(command.VariableName), _userInfo.GetCurrentUserId());
    }
}