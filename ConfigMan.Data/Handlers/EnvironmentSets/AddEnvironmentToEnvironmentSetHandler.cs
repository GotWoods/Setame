using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record AddEnvironmentToEnvironmentSet(Guid EnvironmentSetId, string Name, Guid PerformedBy) : ApplicationCommand(PerformedBy), IRequest;
public class AddEnvironmentToEnvironmentSetHandler : IRequestHandler<AddEnvironmentToEnvironmentSet>
{
    private readonly IDocumentSession _documentSession;

    public AddEnvironmentToEnvironmentSetHandler(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }


    public async Task Handle(AddEnvironmentToEnvironmentSet command, CancellationToken cancellationToken)
    {
        //TODO: Add environment to all Children Applications?
        await _documentSession.AppendToStreamAndSave<EnvironmentSet>(command.EnvironmentSetId, new EnvironmentAdded(command.Name), command.PerformedBy);
    }
}