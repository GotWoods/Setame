using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace ConfigMan.Data.Models.Projections;


public class ActiveApplication
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class ActiveApplicationProjection : SingleStreamProjection<ActiveApplication>
{
    public ActiveApplicationProjection()
    {
        //DeleteEvent<ApplicationDeleted>();
    }

    public ActiveApplication Create(ApplicationCreated created)
    {
        return new ActiveApplication() { Name = created.Name, Id = created.Id };
    }


    public void Apply(ApplicationRenamed e, ActiveApplication current)
    {
        current.Name = e.NewName;
    }


}