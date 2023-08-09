using JasperFx.Core;
using Marten.Events;
using Marten.Events.CodeGeneration;
using Marten.Events.Projections;
using Setame.Data.Models;

namespace Setame.Data.Projections;

public class ApplicationChangeHistoryTransformation : EventProjection
{
    [MartenIgnore]
    public Guid GetUserId<T>(IEvent<T> input) where T : notnull
    {
        var header = input.GetHeader("user-id");
        if (header == null)
            return Guid.Empty;
        return Guid.Parse(header.ToString()!);
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationCreated> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            ApplicationActionType.Create,
            $"Created: '{input.Data.Name}'",
            GetUserId(input)
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationRenamed> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            ApplicationActionType.Create,
            $"Renamed: '{input.Data.NewName}'",
            GetUserId(input)
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationVariableAdded> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            ApplicationActionType.VariableCreate,
            $"Application Variable Created: '{input.Data.Name}'",
            GetUserId(input)
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationVariableChanged> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            ApplicationActionType.VariableChange,
            $"Application Variable {input.Data.VariableName} changed to {input.Data.NewValue} for {input.Data.Environment}",
            GetUserId(input)
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationVariableRenamed> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            ApplicationActionType.VariableRename,
            $"Application Variable {input.Data.VariableName} renamed to {input.Data.NewName}",
            GetUserId(input)
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationDefaultVariableAdded> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            ApplicationActionType.VariableRename,
            $"Application Default {input.Data.VariableName} added",
            GetUserId(input)
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationDefaultVariableChanged> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            ApplicationActionType.VariableRename,
            $"Application Default {input.Data.VariableName} changed to {input.Data.NewValue}",
            GetUserId(input)
        );
    }
}

public record ApplicationChangeHistory(Guid Id, Guid ApplicationId, DateTimeOffset EventTime, ApplicationActionType ApplicationActionType, string Description, Guid User);


public enum ApplicationActionType
{
    Create,
    Delete,
    Rename,
    VariableCreate,

    VariableChange,
    VariableRename
}