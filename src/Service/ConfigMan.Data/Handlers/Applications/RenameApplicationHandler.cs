using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.Applications;

public record RenameApplication(Guid ApplicationId, int ExpectedVersion, string NewName) : IRequest<CommandResponse>;
public class RenameApplicationHandler : IRequestHandler<RenameApplication, CommandResponse>
{
    private readonly IDocumentSessionHelper<Application> _documentSession;
    private readonly IQuerySession _querySession;

    public RenameApplicationHandler(IDocumentSessionHelper<Application> documentSession, IQuerySession querySession)
    {
        _documentSession = documentSession;
        _querySession = querySession;
    }

    public async Task<CommandResponse> Handle(RenameApplication command, CancellationToken cancellationToken)
    {
        var response = new CommandResponse();
        var existing = _querySession.Query<Application>().FirstOrDefault(x => x.Id == command.ApplicationId);
        if (existing != null)
           response.Errors.Add(Errors.ApplicationNotFound(command.ApplicationId));

        await _documentSession.AppendToStream(command.ApplicationId, command.ExpectedVersion, new ApplicationRenamed(command.NewName));
        await _documentSession.SaveChangesAsync();
        return response;
    }
}