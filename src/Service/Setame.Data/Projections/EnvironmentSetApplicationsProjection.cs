using Marten;
using Marten.Events.Projections;
using Setame.Data.Models;

namespace Setame.Data.Projections;

public class EnvironmentSetApplicationAssociation
{
    public Guid Id { get; set; }
    public List<SimpleApplication> Applications { get; set; } = new();
}

public record SimpleApplication(Guid Id, string Name);

public class EnvironmentSetApplicationsProjection : EventProjection
{
    public EnvironmentSetApplicationAssociation Create(EnvironmentSetCreated e)
    {
        return new EnvironmentSetApplicationAssociation { Id = e.Id };
    }

    public async Task Project(ApplicationAssociatedToEnvironmentSet e, IDocumentOperations operations)
    {
        var app = await operations.Events.AggregateStreamAsync<Application>(e.ApplicationId);
        if (app == null)
            throw new NullReferenceException("Application could not be found");

        var existingAssociation = await operations.LoadAsync<EnvironmentSetApplicationAssociation>(e.EnvironmentSetId);
        if (existingAssociation == null)
            return;

        existingAssociation.Applications.Add(new SimpleApplication(app.Id, app.Name));
        operations.Update(existingAssociation);
        operations.Store(existingAssociation);
    }

    //TODO: renames/deletes. Probably need to store the ID of the application in the association
}