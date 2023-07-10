using ConfigMan.Data.Models;
using ConfigMan.Data.Projections;
using Marten;

namespace ConfigMan.Data;

public interface IEnvironmentSetService
{
    Task<List<EnvironmentSet>> GetAll();
    Task<EnvironmentSet> GetOne(Guid environmentSetId);
}

public class EnvironmentSetService : ServiceBase, IEnvironmentSetService
{
    private readonly IQuerySession _querySession;

    public EnvironmentSetService(IDocumentSession documentSession, IQuerySession querySession) : base(documentSession)
    {
        _querySession = querySession;
    }

    public async Task<List<EnvironmentSet>> GetAll()
    {
        var summary = await _querySession.Query<ActiveEnvironmentSet>().ToListAsync();
        var items = new List<EnvironmentSet>();

        foreach (var activeEnvironmentSet in summary)
        {
            var aggregateStreamAsync = await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(activeEnvironmentSet.Id);
            if (aggregateStreamAsync != null)
                items.Add(aggregateStreamAsync);
        }

        var sorted = items.OrderBy(x => x.Name);
        return sorted.ToList();
    }

    public async Task<EnvironmentSet> GetOne(Guid environmentSetId)
    {
        return await _querySession.Events.AggregateStreamAsync<EnvironmentSet>(environmentSetId) ?? throw new NullReferenceException("The environment set could not be found");
    }
}