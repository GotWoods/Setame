﻿using JasperFx.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Setame.Data.Data;
using Setame.Data.Models;

namespace Setame.Data.Handlers.EnvironmentSets;

public record CreateEnvironmentSet(string Name) : IRequest<CommandResponseData<Guid>>;

public class CreateEnvironmentSetHandler : IRequestHandler<CreateEnvironmentSet, CommandResponseData<Guid>>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;
    private readonly ILogger<CreateEnvironmentSetHandler> _logger;


    public CreateEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession,
        IEnvironmentSetRepository environmentSetRepository, ILogger<CreateEnvironmentSetHandler> logger)
    {
        _documentSession = documentSession;
        _environmentSetRepository = environmentSetRepository;
        _logger = logger;
    }

    public async Task<CommandResponseData<Guid>> Handle(CreateEnvironmentSet command,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Creating environment set {Name}", command.Name);
        var matchingName = _environmentSetRepository.GetByName(command.Name);
        if (matchingName != null)
        {
            _logger.LogWarning("Could not create environment set {Name} as an environment set already has that name", command.Name);
            return CommandResponseData<Guid>.FromError(Errors.DuplicateName(command.Name));
        }

        var id = CombGuidIdGeneration.NewGuid();
        _documentSession.Start(id, new EnvironmentSetCreated(id, command.Name));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Environment set created");
        return CommandResponseData<Guid>.FromSuccess(id, 1);
    }
}