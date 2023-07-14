using ConfigMan.Data.Models;
using JasperFx.Core;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

//TODO: change to Request<Guid, ValidationFailed>
public record CreateEnvironmentSet(string Name) : IRequest<Guid>;

public class CreateEnvironmentSetHandler : IRequestHandler<CreateEnvironmentSet, Guid>
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;

    public CreateEnvironmentSetHandler(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }

    public async Task<Guid> Handle(CreateEnvironmentSet command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(command.Name)) throw new ArgumentNullException(nameof(command.Name));

        // var summaryDocument = _querySession.Query<ActiveEnvironmentSet>();
        // if (summaryDocument != null)
        // {
        //     var foundWithSameName = summaryDocument.Environments.Any(x => x.Value == command.Name);
        //     if (foundWithSameName) throw new DuplicateNameException($"The name {command.Name} is already in use");
        // }
        //
        var id = CombGuidIdGeneration.NewGuid();
        _documentSession.SetHeader("user-id", _userInfo.GetCurrentUserId());
        _documentSession.Events.StartStream<EnvironmentSet>(id, new EnvironmentSetCreated(id, command.Name));
        await _documentSession.SaveChangesAsync(cancellationToken);

        return id;
    }
}