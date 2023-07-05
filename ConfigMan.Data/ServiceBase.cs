using Marten;

public abstract class ServiceBase
{
    private readonly IDocumentSession _documentSession;

    protected ServiceBase(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task AppendToStreamAndSave<T>(Guid streamId, object @event, Guid performedBy) where T : class
    {
        _documentSession.SetHeader("user-id", performedBy);
        await _documentSession.Events.WriteToAggregate<T>(streamId, stream => stream.AppendOne(@event));
        await _documentSession.SaveChangesAsync();
    }
}