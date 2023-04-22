using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ConfigMan.ConfigurationProvider;

public class AppLoginRequest
{
    public string ApplicaitonName { get; set; }
    public string Token { get; set; }
}

public class ConfigManConfigurationProvider : Microsoft.Extensions.Configuration.ConfigurationProvider
{
    private readonly HttpClient _httpClient;
    private readonly Uri _serviceUri;

    public ConfigManConfigurationProvider(string application, Uri serviceUri, string clientSecret)
    {
        _serviceUri = serviceUri;
        _httpClient = new HttpClient();

        // var tokenResponse = GetTokenAsync(serviceUri, application, clientSecret).GetAwaiter().GetResult();
        // if (tokenResponse.IsError)
        // {
        //     throw new InvalidOperationException($"Error obtaining token: {tokenResponse.Error}");
        // }

        GetTokenAsync(serviceUri, application, clientSecret).GetAwaiter().GetResult();


        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async Task GetTokenAsync(Uri serviceUri, string clientId, string clientSecret)
    {
        var fullUrl = Path.Combine(_serviceUri + "api/Authentication/AppLogin");
        var request = new AppLoginRequest
        {
            ApplicaitonName = clientId,
            Token = clientSecret
        };
        var jsonData = JsonSerializer.Serialize(request);

        // Create an HttpContent object with the JSON data
        using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(fullUrl, content);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            using JsonDocument jsonDoc = JsonDocument.Parse(json);
            string token = jsonDoc.RootElement.GetProperty("token").GetString();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            throw new Exception($"Failed to fetch settings. Status code: {response.StatusCode}");
        }
        //var discoveryResponse = await client.GetDiscoveryDocumentAsync(serviceUri.ToString());
        // if (discoveryResponse.IsError)
        // {
        //     throw new InvalidOperationException($"Error obtaining discovery document: {discoveryResponse.Error}");
        // }

        // var tokenRequest = new ClientCredentialsTokenRequest
        // {
        //     Address = discoveryResponse.TokenEndpoint,
        //     ClientId = clientId,
        //     ClientSecret = clientSecret,
        //     Scope = "getSettings"
        // };

        //return await client.RequestClientCredentialsTokenAsync(tokenRequest);
    }

    public override void Load()
    {
        LoadSettings().Wait();
    }

    private async Task LoadSettings()
    {
        //var fullUrl = Path.Combine(_serviceUri + "api/ApplicationSettings");
        var fullUrl = "https://localhost:7219/api/ApplicationSettings";
        var response = await _httpClient.GetAsync(fullUrl);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            Data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;
        }
        else
        {
            throw new Exception($"Failed to fetch settings. Status code: {response.StatusCode}");
        }
    }
}