using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record DeleteEnvironmentSet(Guid EnvironmentSetId, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;

public class DeleteEnvironmentSetHandler : IRequestHandler<DeleteEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;

    public DeleteEnvironmentSetHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task Handle(DeleteEnvironmentSet command, CancellationToken cancellationToken)
    {
        await AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentSetDeleted(command.EnvironmentSetId), command.PerformedBy);
    }
}