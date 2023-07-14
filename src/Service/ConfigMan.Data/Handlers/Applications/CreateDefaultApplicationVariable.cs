using ConfigMan.Data.Models;
using Marten;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Handlers.Applications;

public record CreateDefaultApplicationVariable(Guid ApplicationId, string VariableName) : IRequest;

public class CreateDefaultApplicationVariableHandler : IRequestHandler<CreateDefaultApplicationVariable>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public CreateDefaultApplicationVariableHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(CreateDefaultApplicationVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationDefaultVariableAdded(command.VariableName), _userInfo.GetCurrentUserId());
    }
}
