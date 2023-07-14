using ConfigMan.Data.Models;
using Marten.Events.Aggregation;

namespace ConfigMan.Data.Projections;

public class ActiveApplication
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Version { get; set; }
}

public class ActiveApplicationProjection : SingleStreamProjection<ActiveApplication>
{
    public ActiveApplicationProjection()
    {
        DeleteEvent<ApplicationDeleted>();
    }

    public ActiveApplication Create(ApplicationCreated created)
    {
        return new ActiveApplication { Name = created.Name, Id = created.Id };
    }

    public void Apply(ApplicationRenamed e, ActiveApplication current)
    {
        current.Name = e.NewName;
    }
}