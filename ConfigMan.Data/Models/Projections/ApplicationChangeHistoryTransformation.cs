using JasperFx.Core;
using Marten.Events;
using Marten.Events.Projections;

namespace ConfigMan.Data.Models.Projections;

public class ApplicationChangeHistoryTransformation : EventProjection
{
    public ApplicationChangeHistory Transform(IEvent<ApplicationCreated> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            ActionType.Create,
            $"['{input.Timestamp}'] Created: '{input.Data.Name}'",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationRenamed> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            ActionType.Create,
            $"['{input.Timestamp}'] Renamed: '{input.Data.NewName}'",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationVariableAdded> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            ActionType.VariableCreate,
            $"['{input.Timestamp}'] Application Variable Created: '{input.Data.Name}'",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationVariableChanged> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            ActionType.VariableChange,
            $"['{input.Timestamp}'] Application Variable {input.Data.VariableName} changed to {input.Data.NewValue} for {input.Data.Environment}",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    public ApplicationChangeHistory Transform(IEvent<ApplicationVariableRenamed> input)
    {
        return new ApplicationChangeHistory(

            CombGuidIdGeneration.NewGuid(),
            ActionType.VariableRename,
            $"['{input.Timestamp}'] Application Variable {input.Data.VariableName} renamed to {input.Data.NewName}",
            Guid.Parse(input.GetHeader("user-id").ToString())
        );
    }

    //TODO:
    // public record ApplicationDefaultVariableAdded(string VariableName);
    // public record ApplicationDefaultVariableChanged(string VariableName, string NewValue);
}

public record ApplicationChangeHistory(Guid Id, ActionType EnvironmentActionType, string Description, Guid User);


public enum ActionType
{
    Create,
    Delete,
    Rename,
    VariableCreate,

    VariableChange,
    VariableRename
}