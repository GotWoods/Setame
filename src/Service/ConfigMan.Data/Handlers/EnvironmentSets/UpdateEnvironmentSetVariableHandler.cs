using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;
public record UpdateEnvironmentSetVariable(Guid EnvironmentSetId, int ExpectedVersion, string Environment, string VariableName, string VariableValue) : IRequest;

public class UpdateEnvironmentSetVariableHandler : IRequestHandler<UpdateEnvironmentSetVariable>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public UpdateEnvironmentSetVariableHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(UpdateEnvironmentSetVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.ExpectedVersion, command.EnvironmentSetId, new EnvironmentSetVariableChanged(command.Environment, command.VariableName, command.VariableValue), _userInfo.GetCurrentUserId());
    }
}