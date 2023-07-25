using ConfigMan.Data.Projections;
using Marten;

namespace ConfigMan.Data.Data;

public interface IEnvironmentSetApplicationAssociationRepository
{
    EnvironmentSetApplicationAssociation Get(Guid environmentSetId);
}

public class EnvironmentSetApplicationAssociationRepository : IEnvironmentSetApplicationAssociationRepository
{
    private readonly IQuerySession _querySession;

    public EnvironmentSetApplicationAssociationRepository(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public EnvironmentSetApplicationAssociation Get(Guid environmentSetId)
    {
        var associations = _querySession.Query<EnvironmentSetApplicationAssociation>()
            .First(x => x.Id == environmentSetId);
        return associations;
    }
}