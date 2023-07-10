using ConfigMan.Data.Models;
using Marten;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Handlers.Applications;

public record UpdateApplicationVariable(Guid ApplicationId, string Environment, string VariableName, string NewValue) : IRequest;

public class UpdateApplicationVariableHandler : IRequestHandler<UpdateApplicationVariable>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public UpdateApplicationVariableHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(UpdateApplicationVariable command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationVariableChanged(command.Environment, command.VariableName, command.NewValue), _userInfo.GetCurrentUserId());
    }
}