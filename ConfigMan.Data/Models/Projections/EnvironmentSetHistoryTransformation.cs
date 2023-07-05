using JasperFx.Core;
using Marten.Events;
using Marten.Events.Projections;

namespace ConfigMan.Data.Models.Projections;

public class EnvironmentSetHistoryTransformation : EventProjection
{
    

    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentSetRenamed> input)
    {
        return new EnvironmentSetChangeHistory(
            
            CombGuidIdGeneration.NewGuid(),
            EnvironmentActionType.Rename,
            $"['{input.Timestamp}'] Renamed To: '{input.Data.NewName}'",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentSetCreated> input)
    {
        return new EnvironmentSetChangeHistory(
            CombGuidIdGeneration.NewGuid(),
            EnvironmentActionType.Create,
            $"['{input.Timestamp}'] Created: '{input.Data.Name}'",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentSetDeleted> input)
    {
        return new EnvironmentSetChangeHistory(
            CombGuidIdGeneration.NewGuid(),
            EnvironmentActionType.Delete,
            $"['{input.Timestamp}'] Deleted: ",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }
}

public record EnvironmentSetChangeHistory(Guid Id, EnvironmentActionType EnvironmentActionType, string Description, Guid User);


public enum EnvironmentActionType
{
    Create,
    Delete,
    Rename
}