using Marten;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Data.Data;

public interface IEnvironmentSetRepository
{
    Task<EnvironmentSet> GetById(Guid id);
    ActiveEnvironmentSet? GetByName(string name);
}

public class EnvironmentSetRepository : IEnvironmentSetRepository
{
    private readonly IQuerySession _querySession;

    public EnvironmentSetRepository(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<EnvironmentSet> GetById(Guid id)
    {
        var environmentSet = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(id);
        if (environmentSet == null)
            throw new NullReferenceException("Environment Set could not be found with Id of " + id);
        return environmentSet;
    }

    public ActiveEnvironmentSet? GetByName(string name)
    {
        return _querySession.Query<ActiveEnvironmentSet>().FirstOrDefault(x => x.Name == name);
    }
}