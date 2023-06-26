using System.Security.Claims;
using Marten;
using Microsoft.Net.Http.Headers;

namespace ConfigMan.Service
{
    public static class DocumentSessionExtensions
    {
        public static void SetAuthHeader(IDocumentSession documentSession, ClaimsPrincipal claimsPrincipal)
        {
            var claim = claimsPrincipal.FindFirst(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            documentSession.SetHeader("user-id", claim.Value);
        }

        public static async Task Add<T>(this IDocumentSession documentSession, Guid id, object @event, ClaimsPrincipal claimsPrincipal, CancellationToken ct) where T : class
        {
            SetAuthHeader(documentSession, claimsPrincipal);
            documentSession.Events.StartStream<T>(id, @event);
            await documentSession.SaveChangesAsync(token: ct);
        }

        public static Task GetAndUpdate<T>(this IDocumentSession documentSession, Guid id, int version, Func<T, object> handle, ClaimsPrincipal claimsPrincipal, CancellationToken ct) where T : class
        {
            SetAuthHeader(documentSession, claimsPrincipal);
            var writeToAggregate = documentSession.Events.WriteToAggregate<T>(id, stream => stream.AppendOne(handle(stream.Aggregate)), ct);
            return writeToAggregate;
        }
    }
}
