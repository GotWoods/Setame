using ConfigMan.Data.Models;
using Marten;
using MediatR;

namespace ConfigMan.Data.Handlers.EnvironmentSets;

public record GetEnvironment(Guid EnvironmentSetId) : IRequest<EnvironmentSet>;
public class GetEnvironmentHandler : IRequestHandler<GetEnvironment, EnvironmentSet>
{
    private readonly IQuerySession _querySession;

    public GetEnvironmentHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<EnvironmentSet> Handle(GetEnvironment request, CancellationToken cancellationToken)
    {
        //var metadata = await _querySession.Events.FetchStreamStateAsync(request.EnvironmentSetId, cancellationToken);
        return await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(request.EnvironmentSetId, token: cancellationToken) ?? throw new NullReferenceException("The environment set could not be found");
    }
}