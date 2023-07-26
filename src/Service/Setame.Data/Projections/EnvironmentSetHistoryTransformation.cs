using JasperFx.Core;
using Marten.Events;
using Marten.Events.CodeGeneration;
using Marten.Events.Projections;
using Setame.Data.Models;

namespace Setame.Data.Projections;

public class EnvironmentSetHistoryTransformation : EventProjection
{
    [MartenIgnore]
    public Guid GetId<T>(IEvent<T> input) where T : notnull
    {
        var header = input.GetHeader("user-id");
        if (header == null)
            return Guid.Empty;
        return Guid.Parse(header.ToString()!);
    }

    public EnvironmentSetChangeHistory Transform(IEvent<EnvironmentSetRenamed> input)
    {
        return new EnvironmentSetChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            EnvironmentActionType.Rename,
            $"Renamed To: '{input.Data.NewName}'",
            GetId(input)
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
            GetId(input)
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
            GetId(input)
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
            GetId(input)
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
            GetId(input)
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