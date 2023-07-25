using ConfigMan.Data.Models;
using Marten;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.Applications;

public record DeleteApplication(Guid ApplicationId) : IRequest;

public class DeleteApplicationHandler : IRequestHandler<DeleteApplication>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly ILogger<DeleteApplicationHandler> _logger;

    public DeleteApplicationHandler(IDocumentSessionHelper<Application> documentSession, ILogger<DeleteApplicationHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task Handle(DeleteApplication command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.ApplicationId, new ApplicationDeleted(command.ApplicationId));
        await _documentSession.SaveChangesAsync();
    }
}