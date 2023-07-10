using ConfigMan.Data.Models;
using Marten;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Handlers.Applications;

public record CreateApplicationVariable(Guid ApplicationId, string Environment, string VariableName) : IRequest;

public class CreateApplicationVariableHandler : IRequestHandler<CreateApplicationVariable>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public CreateApplicationVariableHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(CreateApplicationVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationVariableAdded(command.Environment, command.VariableName), _userInfo.GetCurrentUserId());
    }
}

