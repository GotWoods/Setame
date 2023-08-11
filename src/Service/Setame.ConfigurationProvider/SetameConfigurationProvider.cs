using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Setame.ConfigurationProvider;

public class SetameConfigurationProvider : Microsoft.Extensions.Configuration.ConfigurationProvider
{
    private readonly HttpClient _httpClient;
    private readonly Uri _serviceUri;

    public SetameConfigurationProvider(string application, Uri serviceUri, string environment,  string clientToken)
    {
        _serviceUri = serviceUri;
        _httpClient = new HttpClient();

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        GetTokenAsync(serviceUri, application, environment, clientToken).GetAwaiter().GetResult();
    }

    private async Task GetTokenAsync(Uri serviceUri, string clientId, string environment, string clientToken)
    {
        var fullUrl = $"{serviceUri}api/Authentication/AppLogin";
        var request = new AppLoginRequest
        {
            ApplicationName = clientId,
            Token = clientToken,
            Environment = environment
        };
        var jsonData = JsonSerializer.Serialize(request);

        // Create an HttpContent object with the JSON data
        using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(fullUrl, content);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(json);
            var token = jsonDoc.RootElement.GetProperty("token").GetString();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            throw new Exception($"Failed to fetch settings. Status code: {response.StatusCode}");
        }
    }

    public override void Load()
    {
        LoadSettings().Wait();
    }

    private async Task LoadSettings()
    {
        //var fullUrl = Path.Combine(_serviceUri + "api/ApplicationSettings");
        var fullUrl = $"{_serviceUri}api/Client";
        var response = await _httpClient.GetAsync(fullUrl);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            Data = JsonConvert.DeserializeObject<LoggingDictionary<string, string>>(json)!;
        }
        else
        {
            throw new Exception($"Failed to fetch settings. Status code: {response.StatusCode}");
        }
    }

    // public override string Get(string key)
    // {
    //     // Implement the tracking logic here
    //     Console.WriteLine($"Accessed configuration value for key: {key}");
    //
    //     return base.Get(key);
    // }
}