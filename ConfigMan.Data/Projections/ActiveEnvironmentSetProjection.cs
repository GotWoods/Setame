using ConfigMan.Data.Models;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace ConfigMan.Data.Projections;

public class ActiveEnvironmentSet
{
    public Guid Id { get; set; } //this will always be Guid.Empty as we only want 1 summary
    public string Name { get; set; }
}

public class ActiveEnvironmentSetProjection : SingleStreamProjection<ActiveEnvironmentSet>
{
    public ActiveEnvironmentSetProjection()
    {
        DeleteEvent<EnvironmentSetDeleted>();
    }

    public ActiveEnvironmentSet Create(EnvironmentSetCreated created)
    {
        return new ActiveEnvironmentSet() { Name = created.Name, Id = created.Id };
    }

    public void Apply(EnvironmentSetRenamed e, ActiveEnvironmentSet current)
    {
        current.Name = e.NewName;
    }

    // public void Apply(EnvironmentSetDeleted e, ActiveEnvironmentSet current)
    // {
    //     current.Environments.Remove(e.Id);
    // }

    // public void Apply(EnvironmentSetCreated e, ActiveEnvironmentSet current)
    // {
    //     current.Environments.Add(e.Id, e.Name);
    // }

    // public void Apply(EnvironmentSetRenamed e, ActiveEnvironmentSet current)
    // {
    //     current.Environments[e.Id] = e.NewName;
    // }
}