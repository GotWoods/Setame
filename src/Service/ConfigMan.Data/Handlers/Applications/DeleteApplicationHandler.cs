using ConfigMan.Data.Models;
using Marten;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMan.Data.Handlers.Applications;

public record DeleteApplication(Guid ApplicationId) : IRequest;

public class DeleteApplicationHandler : IRequestHandler<DeleteApplication>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public DeleteApplicationHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task Handle(DeleteApplication command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStreamAndSave<Application>(command.ApplicationId, new ApplicationDeleted(command.ApplicationId), _userInfo.GetCurrentUserId());
    }
}