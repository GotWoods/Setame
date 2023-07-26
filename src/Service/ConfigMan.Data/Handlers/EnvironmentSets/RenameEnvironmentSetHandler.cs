using ConfigMan.Data.Data;
using ConfigMan.Data.Handlers.Applications;
using ConfigMan.Data.Models;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string NewName) : IRequest<CommandResponse>;

public class RenameEnvironmentSetHandler : IRequestHandler<RenameEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;
    private readonly ILogger<RenameEnvironmentSetHandler> _logger;

    public RenameEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IEnvironmentSetRepository environmentSetRepository, ILogger<RenameEnvironmentSetHandler> logger)
    {
        _documentSession = documentSession;
        _environmentSetRepository = environmentSetRepository;
        _logger = logger;
    }

    public async Task<CommandResponse> Handle(RenameEnvironmentSet command, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Renaming {EnvironmentSet} to {NewName}", command.EnvironmentSetId, command.NewName);
        var environmentSet = _environmentSetRepository.GetByName(command.NewName);
        if (environmentSet != null)
        {
            _logger.LogWarning("Could not rename to {NewName} as an environment set already has that name", command.NewName);
            return CommandResponse.FromError(Errors.DuplicateName(command.NewName));
        }
        
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetRenamed(command.EnvironmentSetId, command.NewName));
        await _documentSession.SaveChangesAsync();
        _logger.LogDebug("Environment set renamed");
        return CommandResponse.FromSuccess(command.ExpectedVersion +1);
    }
}