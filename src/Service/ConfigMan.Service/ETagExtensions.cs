using Microsoft.Net.Http.Headers;

namespace ConfigMan.Service;

public static class ETagExtensions
{
    public static int GetIfMatchRequestHeader(this HttpRequest request)
    {
        var header = request.GetTypedHeaders().IfMatch.FirstOrDefault();
        if (header is null) 
            throw new NullReferenceException("If-Match header was null");

        return int.Parse(header.Tag.Substring(1, header.Tag.Length - 2)); //remove the quotes
    }

    public static void TrySetETagResponseHeader(this HttpResponse response, object etag)
    {
        if (!response.IsSuccessful()) return;

        response.GetTypedHeaders().ETag = new EntityTagHeaderValue($"\"{etag}\"", true);
    }
}