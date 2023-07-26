namespace Setame.Web.Models
{
    public record ChangeHistory(DateTimeOffset timestamp, string actionType, string Description, string User);
}
