using JasperFx.Core;
using Marten.Events;
using Marten.Events.Projections;

namespace ConfigMan.Data.Models.Projections;

public class ApplicationChangeHistoryTransformation : EventProjection
{
    public ApplicationChangeHistory Transform(IEvent<ApplicationCreated> input)
    {
        var userHeader = input.GetHeader("user-id");
        var userId = Guid.Empty;
        if (userHeader != null)
            userId = Guid.Parse((string)userHeader);

        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            input.StreamId,
            input.Timestamp,
            ApplicationActionType.Create,
            $"Created: '{input.Data.Name}'",
            userId
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
            Guid.Parse(input.GetHeader("user-id").ToString())
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
            Guid.Parse(input.GetHeader("user-id").ToString())
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
            Guid.Parse(input.GetHeader("user-id").ToString())
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
            Guid.Parse(input.GetHeader("user-id").ToString())
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
            Guid.Parse(input.GetHeader("user-id").ToString())
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
            Guid.Parse(input.GetHeader("user-id").ToString())
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