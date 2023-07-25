﻿using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentSet(Guid EnvironmentSetId) : IRequest;

public class DeleteEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentSet>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly ILogger<DeleteEnvironmentSetHandler> _logger;

    public DeleteEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, ILogger<DeleteEnvironmentSetHandler> logger)
    {
        _documentSession = documentSession;
        _logger = logger;
    }

    public async Task Handle(DeleteEnvironmentSet command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.EnvironmentSetId, new EnvironmentSetDeleted(command.EnvironmentSetId));
        await _documentSession.SaveChangesAsync();
    }
}