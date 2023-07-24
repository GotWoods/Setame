using ConfigMan.Data.Data;
using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record RenameEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string NewName) : IRequest<CommandResponse>;

public class RenameEnvironmentSetHandler : IRequestHandler<RenameEnvironmentSet, CommandResponse>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;
    private readonly IEnvironmentSetRepository _environmentSetRepository;

    public RenameEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession, IEnvironmentSetRepository environmentSetRepository)
    {
        _documentSession = documentSession;
        _environmentSetRepository = environmentSetRepository;
    }

    public async Task<CommandResponse> Handle(RenameEnvironmentSet command, CancellationToken cancellationToken)
    {
        var environmentSet = _environmentSetRepository.GetByName(command.NewName);
        if (environmentSet != null)
            return CommandResponse.FromError(Errors.DuplicateName(command.NewName));

        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentSetRenamed(command.EnvironmentSetId, command.NewName));
        await _documentSession.SaveChangesAsync();
        return CommandResponse.FromSuccess(command.ExpectedVersion +1);
    }
}