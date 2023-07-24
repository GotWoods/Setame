using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;

namespace ConfigMan.Data.Data;

public interface IEnvironmentSetRepository
{
    Task<EnvironmentSet?> GetById(Guid id);
    EnvironmentSet? GetByName(string name);
}

public class EnvironmentSetRepository : IEnvironmentSetRepository
{
    private readonly IQuerySession _querySession;

    public EnvironmentSetRepository(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<EnvironmentSet?> GetById(Guid id)
    {
        return await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(id);
    }

    public EnvironmentSet? GetByName(string name)
    {
        //TODO: this should only be active environments that are queried 
        return _querySession.Query<EnvironmentSet>().FirstOrDefault(x => x.Name == name);
    }
}