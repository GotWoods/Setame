using Marten;

namespace Setame.Data;

public interface IDocumentSessionHelper<T> where T : class
{
    void Start(Guid id, params object[] @event);
    Task<T?> GetFromEventStream(Guid id);
    Task AppendToStream(Guid streamId, object @event);
    Task AppendToStream(Guid streamId, int expectedVersion, object @event);
    Task AppendToStreamAnonymously(Guid streamId, object @event);
    Task SaveChangesAsync();
}

public class DocumentSessionHelper<T> : IDocumentSessionHelper<T> where T : class 
{
    private readonly IDocumentSession _documentSession;
    private readonly IUserInfo _userInfo;


    public DocumentSessionHelper(IDocumentSession documentSession, IUserInfo userInfo)
    {
        _documentSession = documentSession;
        _userInfo = userInfo;
    }


    public void Start(Guid id, params object[] @event)
    {
        _documentSession.SetHeader("user-id", _userInfo.GetCurrentUserId());
        _documentSession.Events.StartStream<T>(id, @event);
    }

    public async Task<T?> GetFromEventStream(Guid id)
    {
        return await _documentSession.Events.AggregateStreamAsync<T>(id);
    }
    
    public Task AppendToStream(Guid streamId, object @event)
    {
        _documentSession.SetHeader("user-id", _userInfo.GetCurrentUserId());
        _documentSession.Events.Append(streamId, @event);
        return Task.CompletedTask;
    }

    public Task AppendToStreamAnonymously(Guid streamId, object @event)
    {
        _documentSession.Events.Append(streamId, @event);
        return Task.CompletedTask;
    }

    public async Task AppendToStream(Guid streamId, int expectedVersion, object @event)
    {
        _documentSession.SetHeader("user-id", _userInfo.GetCurrentUserId());
        await _documentSession.Events.WriteToAggregate<T>(streamId, expectedVersion, stream => stream.AppendOne(@event));
    }

    public async Task SaveChangesAsync()
    {
        await _documentSession.SaveChangesAsync();
    }
}