using Marten.Events.Projections;

namespace ConfigMan.Data.Models.Projections;

public class EnvironmentSetSummary
{
    public Guid Id { get; set; } //this will always be Guid.Empty as we only want 1 summary
    public Dictionary<Guid, string> Environments { get; set; } = new();
}

public class EnvironmentSetSummaryProjection : MultiStreamProjection<EnvironmentSetSummary, Guid>
{
    public EnvironmentSetSummaryProjection()
    {
        Identity<EnvironmentSetCreated>(_=>Guid.Empty); //we are aggregating this all into one single document with a blank ID
        Identity<EnvironmentSetRenamed>(_=>Guid.Empty);
        Identity<EnvironmentSetDeleted>(_=>Guid.Empty);
    }

    public void Apply(EnvironmentSetDeleted e, EnvironmentSetSummary current)
    {
        current.Environments.Remove(e.Id);
    }

    public void Apply(EnvironmentSetCreated e, EnvironmentSetSummary current)
    {
        current.Environments.Add(e.Id, e.Name);
    }

    public void Apply(EnvironmentSetRenamed e, EnvironmentSetSummary current)
    {
        current.Environments[e.Id] = e.NewName;
    }
}

