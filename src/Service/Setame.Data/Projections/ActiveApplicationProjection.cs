using Marten.Events.Aggregation;
using Setame.Data.Models;

namespace Setame.Data.Projections;

public class ActiveApplication
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Version { get; set; }

    public string Token { get; set; } = string.Empty;
}

public class ActiveApplicationProjection : SingleStreamProjection<ActiveApplication>
{
    public ActiveApplicationProjection()
    {
        DeleteEvent<ApplicationDeleted>();
    }

    public ActiveApplication Create(ApplicationCreated created)
    {
        return new ActiveApplication { Name = created.Name, Id = created.Id, Token = created.Token };
    }

    public void Apply(ApplicationRenamed e, ActiveApplication current)
    {
        current.Name = e.NewName;
    }

    //subscribe to all application events so version matches underlying changes
    public void Apply(IApplicationEvent e, ActiveApplication current)
    {

    }
}