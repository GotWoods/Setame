using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace ConfigMan.Service.Tests.Functional;

public class ApiTestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // You can customize the configuration and services here if needed
        // For example, you can replace the database with an in-memory database for testing.
    }

    public new async Task<HttpClient> CreateAuthorizedClientAsync()
    {
        var client = base.CreateClient();
        var jsonContent = new StringContent(JsonConvert.SerializeObject(new
        {
            Username = "dave@solidhouse.com",
            Password = "admin"
        }), Encoding.UTF8, "application/json");

        var createResponse = await client.PostAsync("/api/Authentication/login", jsonContent);

        var jsonResult = await createResponse.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<LoginResult>(jsonResult);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Token);
        return client;
    }
}