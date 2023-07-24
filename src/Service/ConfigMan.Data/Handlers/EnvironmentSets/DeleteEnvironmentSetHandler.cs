using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentSet(Guid EnvironmentSetId) : IRequest;

public class DeleteEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentSet>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;

    public DeleteEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(DeleteEnvironmentSet command, CancellationToken cancellationToken)
    {
        await _documentSession.AppendToStream(command.EnvironmentSetId, -1, new EnvironmentSetDeleted(command.EnvironmentSetId));
        await _documentSession.SaveChangesAsync();
    }
}