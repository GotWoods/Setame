using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record AddEnvironmentToEnvironmentSet(Guid EnvironmentSetId, int ExpectedVersion, string Name) : IRequest;
public class AddEnvironmentToEnvironmentSetHandler : IRequestHandler<AddEnvironmentToEnvironmentSet>
{
    private readonly IDocumentSessionHelper<EnvironmentSet> _documentSession;

    public AddEnvironmentToEnvironmentSetHandler(IDocumentSessionHelper<EnvironmentSet> documentSession)
    {
        _documentSession = documentSession;
    }
    
    public async Task Handle(AddEnvironmentToEnvironmentSet command, CancellationToken cancellationToken)
    {
        //TODO: Add environment to all Children Applications?
        //TODO: ensure no duplicates
        await _documentSession.AppendToStream(command.EnvironmentSetId, command.ExpectedVersion, new EnvironmentAdded(command.Name));
    }
}