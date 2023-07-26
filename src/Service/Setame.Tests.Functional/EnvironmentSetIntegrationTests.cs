using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using Setame.Data.Models;

namespace Setame.Tests.Functional;

public class EnvironmentSetIntegrationTests : IClassFixture<ApiTestWebApplicationFactory>
{
    private readonly HttpClient _client;


    public EnvironmentSetIntegrationTests(ApiTestWebApplicationFactory factory)
    {
        _client = factory.CreateAuthorizedClientAsync().Result;
    }

    [Fact]
    public async Task HappyPath_EnvironmentSet()
    {
        var originalName = "Test Environment Set" + DateTime.Now.Ticks.ToString();
        var createResponse = await _client.PostAsync("/api/EnvironmentSets", SerializeContent(originalName));
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


            await AddEnvironment(environmentSet, "Dev");
            environmentSet = await GetEnvironmentSet(environmentSetId);
            Assert.Equal(3, environmentSet.Version);
            Assert.Single(environmentSet.DeploymentEnvironments);
            Assert.Equal("Dev", environmentSet.DeploymentEnvironments[0].Name);

            await AddEnvironment(environmentSet, "Stage");
            environmentSet = await GetEnvironmentSet(environmentSetId);
            Assert.Equal(4, environmentSet.Version);
            Assert.Equal(2, environmentSet.DeploymentEnvironments.Count);
            Assert.Equal("Dev", environmentSet.DeploymentEnvironments[0].Name);
            Assert.Equal("Stage", environmentSet.DeploymentEnvironments[1].Name);


            await RenameEnvironment(environmentSet, "Stage", "Staging");
            environmentSet = await GetEnvironmentSet(environmentSetId);
            Assert.Equal(5, environmentSet.Version);
            Assert.Equal(2, environmentSet.DeploymentEnvironments.Count);
            Assert.Equal("Dev", environmentSet.DeploymentEnvironments[0].Name);
            Assert.Equal("Staging", environmentSet.DeploymentEnvironments[1].Name);

            await DeleteEnvironment(environmentSet, "Staging");
            environmentSet = await GetEnvironmentSet(environmentSetId);
            Assert.Equal(6, environmentSet.Version);
            Assert.Equal(1, environmentSet.DeploymentEnvironments.Count);
            Assert.Equal("Dev", environmentSet.DeploymentEnvironments[0].Name);
            

           

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
        SetVersionHeader(environmentSet.Version);
        var renameEnvironmentSet = await _client.PutAsync("/api/EnvironmentSets/" + environmentSet.Id + "/rename",
            SerializeContent(newName));
        renameEnvironmentSet.EnsureSuccessStatusCode();
    }

    private async Task AddEnvironment(EnvironmentSet environmentSet, string newEnvironmentName)
    {
        SetVersionHeader(environmentSet.Version);
        var renameEnvironmentSet = await _client.PostAsync($"/api/EnvironmentSets/{environmentSet.Id}/environment/", SerializeContent(newEnvironmentName));
        renameEnvironmentSet.EnsureSuccessStatusCode();
    }

    private async Task RenameEnvironment(EnvironmentSet environmentSet, string oldEnvironmentName, string newEnvironmentName)
    {
        SetVersionHeader(environmentSet.Version);
        var renameEnvironmentSet = await _client.PutAsync($"/api/EnvironmentSets/{environmentSet.Id}/environment/{oldEnvironmentName}/rename", SerializeContent(newEnvironmentName));
        renameEnvironmentSet.EnsureSuccessStatusCode();
    }

    private async Task DeleteEnvironment(EnvironmentSet environmentSet, string environmentName)
    {
        SetVersionHeader(environmentSet.Version);
        var renameEnvironmentSet = await _client.DeleteAsync($"/api/EnvironmentSets/{environmentSet.Id}/environment/{environmentName}");
        renameEnvironmentSet.EnsureSuccessStatusCode();
    }

    private async Task<EnvironmentSet> GetEnvironmentSet(string environmentSetId)
    {
        var getEnvironmentSet = await _client.GetAsync("/api/EnvironmentSets/" + environmentSetId);
        getEnvironmentSet.EnsureSuccessStatusCode();

        var environmentSet = await getEnvironmentSet.Content.ReadFromJsonAsync<EnvironmentSet>();
        return environmentSet!;
    }


    private StringContent SerializeContent(object content)
    {
        return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
    }

    private void SetVersionHeader(long version)
    {
        _client.DefaultRequestHeaders.IfMatch.Clear();
        _client.DefaultRequestHeaders.IfMatch.Add(new EntityTagHeaderValue("\"" + version + "\""));
    }

}

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
}