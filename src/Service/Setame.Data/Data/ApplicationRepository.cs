using Marten;
using Setame.Data.Models;
using Setame.Data.Projections;

namespace Setame.Data.Data;

public interface IApplicationRepository
{
    ActiveApplication? GetByName(string name);
    Task<Application> GetById(Guid id);
    List<ActiveApplication> GetAllActive();
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

    public List<ActiveApplication> GetAllActive()
    {
        return _querySession.Query<ActiveApplication>().ToList();
    }

    public async Task<Application> GetById(Guid id)
    {
        var application = await _querySession.Events.AggregateStreamAsync<Application>(id);
        if (application == null)
            throw new NullReferenceException("Application could not be found via Id of " + id);
        return application;
    }
}