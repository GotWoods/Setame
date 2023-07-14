using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record UpdateDefaultApplicationVariable(Guid ApplicationId, int ExpectedVersion, string VariableName, string NewValue) : IRequest;
public class UpdateDefaultApplicationVariableHandler : IRequestHandler<UpdateDefaultApplicationVariable>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public UpdateDefaultApplicationVariableHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(UpdateDefaultApplicationVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<Application>(command.ExpectedVersion, command.ApplicationId, new ApplicationDefaultVariableChanged(command.VariableName, command.NewValue), _userInfo.GetCurrentUserId());
    }
}