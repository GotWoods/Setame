using System.Net.Http.Headers;
using ConfigMan.Data.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Json;

namespace ConfigMan.Service.Tests.Functional;

public class EnvironmentSetIntegrationTests : IClassFixture<ApiTestWebApplicationFactory>
{
    private readonly ApiTestWebApplicationFactory _factory;
    private HttpClient _client;


    public EnvironmentSetIntegrationTests(ApiTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateAuthorizedClientAsync().Result;
    }

    [Fact]
    public async Task CreateEnvironmentSet()
    {
        var originalName = "Test Environment Set" + DateTime.Now.Ticks.ToString();
        var createResponse = await _client.PostAsync("/api/EnvironmentSets", SerializeContent(new 
        {
            Name = originalName,
        }));
        createResponse.EnsureSuccessStatusCode();
        
        var environmentSetId = await createResponse.Content.ReadAsStringAsync();
        environmentSetId = environmentSetId.Replace("\"", "");
        
        try
        {
            var environmentSet = await GetEnvironmentSet(environmentSetId);
            Assert.Equal(1, environmentSet.Version);
            Assert.Equal(originalName, environmentSet.Name);
            await RenameEnvironmentSet(environmentSet);

            environmentSet = await GetEnvironmentSet(environmentSetId);
            Assert.Equal(2, environmentSet.Version);
            Assert.Equal("newName", environmentSet.Name);



        }
        finally //always try to cleanup our mess
        {
            await DeleteEnvironmentSet(environmentSetId);
        }
    }

    private async Task DeleteEnvironmentSet(string environmentSetId)
    {
        var deleteEnvironmentSet = await _client.DeleteAsync("/api/EnvironmentSets/" + environmentSetId);
        deleteEnvironmentSet.EnsureSuccessStatusCode();
    }

    private async Task RenameEnvironmentSet(EnvironmentSet environmentSet)
    {
        var newName = "newName"; // Replace with the actual new name
        _client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue("\""+ environmentSet.Version +"\""));
        var renameEnvironmentSet = await _client.PutAsync("/api/EnvironmentSets/" + environmentSet.Id + "/rename",
            SerializeContent(newName));
        renameEnvironmentSet.EnsureSuccessStatusCode();
    }

    private async Task<EnvironmentSet> GetEnvironmentSet(string environmentSetId)
    {
        var getEnvironmentSet = await _client.GetAsync("/api/EnvironmentSets/" + environmentSetId);
        getEnvironmentSet.EnsureSuccessStatusCode();

        var environmentSet = await getEnvironmentSet.Content.ReadFromJsonAsync<EnvironmentSet>();
        return environmentSet!;
    }


    public StringContent SerializeContent(object content)
    {
        return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
    }
}

public class LoginResult
{
    public string Token { get; set; }
}