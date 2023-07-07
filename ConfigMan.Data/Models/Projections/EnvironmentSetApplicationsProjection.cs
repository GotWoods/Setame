using Marten;
using Marten.Events;
using Marten.Events.Projections;

namespace ConfigMan.Data.Models.Projections;

public class EnvironmentSetApplicationAssociation
{
    public Guid Id { get; set; }
    public List<string> Applications { get; set; } = new();
}

public class EnvironmentSetApplicationsProjection : EventProjection
{
    public EnvironmentSetApplicationAssociation Create(EnvironmentSetCreated e)
    {
        return new EnvironmentSetApplicationAssociation { Id = e.Id };
    }

    public async Task Project(IEvent<ApplicationAssociatedToEnvironmentSet> e, IDocumentOperations operations)
    {
        var app = await operations.Events.AggregateStreamAsync<Application>(e.Data.ApplicationId);
        var existingAssociation = await operations.LoadAsync<EnvironmentSetApplicationAssociation>(e.Data.EnvironmentSetId);
        existingAssociation.Applications.Add(app.Name);
        operations.Update(existingAssociation);
        operations.Store(existingAssociation);
    }
}