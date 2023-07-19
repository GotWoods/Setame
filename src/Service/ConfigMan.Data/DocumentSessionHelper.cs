using ConfigMan.Data.Models;
using Marten;

namespace ConfigMan.Data;

public interface IDocumentSessionHelper<T> where T : class
{
    void Start(Guid id, params object[] @event);
    Task<T?> GetFromEventStream(Guid id);
    Task AppendToStream<T>(Guid streamId, object @event) where T : class;
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
    
    public async Task AppendToStream<T>(Guid streamId, object @event) where T : class
    {
        _documentSession.SetHeader("user-id", _userInfo);
        _documentSession.Events.Append(streamId, @event);
        //await _documentSession.Events.WriteToAggregate<T>(streamId, stream => stream.AppendOne(@event));
//        await _documentSession.SaveChangesAsync();
    }

    public static async Task AppendToStreamAndSave<T>(IDocumentSession documentSession, int expectedVersion, Guid streamId, object @event, Guid performedBy) where T : class
    {
        documentSession.SetHeader("user-id", performedBy);
        await documentSession.Events.WriteToAggregate<T>(streamId, expectedVersion, stream => stream.AppendOne(@event));
  //      await documentSession.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _documentSession.SaveChangesAsync();
    }
}