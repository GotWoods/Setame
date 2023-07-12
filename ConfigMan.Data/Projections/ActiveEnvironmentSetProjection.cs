using ConfigMan.Data.Models;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace ConfigMan.Data.Projections;

public class ActiveEnvironmentSet
{
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public int Version { get; set; }
}

public class ActiveEnvironmentSetProjection : SingleStreamProjection<ActiveEnvironmentSet>
{
    public ActiveEnvironmentSetProjection()
    {
        DeleteEvent<EnvironmentSetDeleted>();
    }

    public ActiveEnvironmentSet Create(EnvironmentSetCreated created)
    {
        return new ActiveEnvironmentSet() { Name = created.Name, Id = created.Id, Version = 1};
    }

    public void Apply(EnvironmentSetRenamed e, ActiveEnvironmentSet current)
    {
        current.Name = e.NewName;
    }
}