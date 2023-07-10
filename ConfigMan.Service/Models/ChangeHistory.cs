namespace ConfigMan.Service.Models
{
    public record ChangeHistory(DateTimeOffset timestamp, string actionType, string Description, string User);
}
