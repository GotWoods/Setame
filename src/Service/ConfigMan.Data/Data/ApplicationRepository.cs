using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;

namespace ConfigMan.Data.Data;

public interface IApplicationRepository
{
    ActiveApplication? GetByName(string name);
    Task<Application?> GetById(Guid id);
}

public class ApplicationRepository : IApplicationRepository
{
    private readonly IQuerySession _querySession;

    public ApplicationRepository(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public ActiveApplication? GetByName(string name)
    {
        return _querySession.Query<ActiveApplication>().FirstOrDefault(x => x.Name == name);
    }

    public async Task<Application?> GetById(Guid id)
    {
        return await _querySession.Events.AggregateStreamAsync<Application>(id);
    }
}