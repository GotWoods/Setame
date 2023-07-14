using ConfigMan.Data.Models;
using JasperFx.Core;
using Marten.Events;
using Marten.Events.Projections;

namespace ConfigMan.Data.Projections;

public class EnvironmentSetHistoryTransformation : EventProjection
{
    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentSetRenamed> input)
    {
        return new EnvironmentSetChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            EnvironmentActionType.Rename,
            $"Renamed To: '{input.Data.NewName}'",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentSetCreated> input)
    {
        return new EnvironmentSetChangeHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            EnvironmentActionType.Create,
            $"Created: '{input.Data.Name}'",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentSetDeleted> input)
    {
        return new EnvironmentSetChangeHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            EnvironmentActionType.Delete,
            $"Deleted: ",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentAdded> input)
    {
        return new EnvironmentSetChangeHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            EnvironmentActionType.EnvironmentAdded,
            $"Environment Added: {input.Data.Name}",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentRenamed> input)
    {
        return new EnvironmentSetChangeHistory(
            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            EnvironmentActionType.Rename,
            $"Environment Renamed from {input.Data.OldName} to {input.Data.NewName}",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }
}

public record EnvironmentSetChangeHistory(Guid Id, Guid EnvironmentSetId, DateTimeOffset timestamp, EnvironmentActionType EnvironmentActionType, string Description, Guid User);


public enum EnvironmentActionType
{
    Create,
    Delete,
    Rename,
    EnvironmentAdded

}