using Marten;

public static class DocumentSessionExtensions
{
    public static async Task AppendToStreamAndSave<T>(this IDocumentSession documentSession, Guid streamId, object @event, Guid performedBy) where T : class
    {
        documentSession.SetHeader("user-id", performedBy);
        await documentSession.Events.WriteToAggregate<T>(streamId, stream => stream.AppendOne(@event));
        await documentSession.SaveChangesAsync();
    }
}