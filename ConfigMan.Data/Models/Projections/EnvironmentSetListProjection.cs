using Marten.Events;
using Marten;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Marten.Internal.Sessions;

namespace ConfigMan.Data.Models.Projections;

public class EnvironmentSetSummary
{
    public Guid Id { get; set; }
    public Dictionary<Guid, string> Environments { get; set; } = new();
}

public class EnvironmentSetSummaryProjection : SingleStreamProjection<EnvironmentSetSummary>
{
    // public static EnvironmentSetSummary Create()
    // {
    //     return new EnvironmentSetSummary();
    // }

    public void Apply(EnvironmentSetCreated e, EnvironmentSetSummary current)
    {
        current.Environments.Add(e.Id, e.Name);
    }

    public void Apply(EnvironmentSetRenamed e, EnvironmentSetSummary current)
    {
        current.Environments[e.Id] = e.NewName;
    }

    public void Apply(EnvironmentSetDeleted e, EnvironmentSetSummary current)
    {
        current.Environments.Remove(e.Id);
    }
}

public class CustomerIncidentsSummaryGrouper : IAggregateGrouper<Guid>
{
    private readonly Type[] eventTypes =
    {
        typeof(EnvironmentSetCreated), typeof(EnvironmentSetRenamed), typeof(EnvironmentSetDeleted)
    };

    public async Task Group(IQuerySession session, IEnumerable<IEvent> events, ITenantSliceGroup<Guid> grouping)
    {
        var filteredEvents = events
            .Where(ev => eventTypes.Contains(ev.EventType))
            .ToList();

        if (!filteredEvents.Any())
            return;

        var documentSetIds = filteredEvents.Select(e => e.StreamId).ToList();
        
        // var result = await session.Query<EnvironmentSet>()
        //     .Where(x => documentSetIds.Contains(x.Id))
        //     .Select(x => new { x.Id, x.Name })
        //     .ToListAsync();

        // var streamIds = (IDictionary<Guid, List<Guid>>)result.GroupBy(ks => ks.Id, vs => vs.Id)
        //     .ToDictionary(ks => ks.Key, vs => vs.ToList());

        //grouping.AddEvents<LicenseFeatureToggled>(e => streamIds[e.LicenseId], licenceFeatureTogglesEvents);

        // var result = await session.Events.QueryRawEventDataOnly<EnvironmentSetCreated>()
        //     .Where(e => documentSetIds.Contains(e.IncidentId))
        //     .Select(x => new { x.IncidentId, x.CustomerId })
        //     .ToListAsync();

        // foreach (var group in result.Select(g => new { g.CustomerId, Events = filteredEvents.Where(ev => ev.StreamId == g.IncidentId) }))
        // {
        //     grouping.AddEvents(group.CustomerId, group.Events);
        // }
    }


    public class Custom : CustomProjection<EnvironmentSetSummary, Guid>
    {
        public override ValueTask ApplyChangesAsync(DocumentSessionBase session, EventSlice<EnvironmentSetSummary, Guid> slice, CancellationToken cancellation, ProjectionLifecycle lifecycle = ProjectionLifecycle.Inline)
        {
            var aggregate = slice.Aggregate;
            foreach (var data in slice.AllData())
                switch (data)
                {
                    case EnvironmentSetCreated:
                        {
                            aggregate = new EnvironmentSetSummary();
                            //set an ID here
                            break;
                        }
                    case EnvironmentSetRenamed e:
                        {
                            //aggregate.Environments[] = e.NewName
                            break;
                        }
                    case EnvironmentSetDeleted:
                        {
                            session.Delete(aggregate);
                            //aggregate.Deleted = true;
                            break;
                        }
                }

            // Apply any updates!
            if (aggregate != null) session.Store(aggregate);

            return new ValueTask();
        }
    }
}