using Setame.ConfigurationProvider;

namespace Setame.Tests.Functional;

public class ProviderTests
{
    // [Fact]
    // public async Task Test1()
    // {
    //     using var host = await new HostBuilder()
    //         .ConfigureWebHost(webBuilder =>
    //         {
    //             webBuilder
    //                 .UseTestServer()
    //                 .ConfigureAppConfiguration(x => x.AddSetame())
    //                 .ConfigureServices(services =>
    //                 {
    //                     //services.AddMyServices();
    //                 })
    //                 .Configure(app =>
    //                 {
    //                     //app.UseMiddleware<MyMiddleware>();
    //                 });
    //         })
    //         .StartAsync();
    // // }
    // class MyWebApplication : WebApplicationFactory<ConfigMan.Service.Program>
    // {
    //     protected override IHost CreateHost(IHostBuilder builder)
    //     {
    //         // shared extra set up goes here
    //         return base.CreateHost(builder);
    //     }
    // }

    [Fact]
    public void ProviderLoadsSettingsCorrectly()
    {
        //var server = new TestServer(new WebHostBuilder().UseStartup<ConfigMan.Service.>());
        

        var applicationId = "Email Service";
        var apiUrl = new Uri("https://localhost:7219/");

       var configManConfigurationProvider = new SetameConfigurationProvider(applicationId, apiUrl, "fqgfa2msm0");
        
        // Act
        configManConfigurationProvider.Load();

        
        // Assert
        string? configValue;
        var success = configManConfigurationProvider.TryGet("LoggingUri", out configValue);
        Assert.True(success);
        Assert.NotNull(configValue);
        Assert.Equal("https://seq.localhost.com", configValue);
    }

    // private class TestHttpClientFactory : IHttpClientFactory
    // {
    //     public HttpClient CreateClient(string name = null)
    //     {
    //         return new HttpClient();
    //     }
    // }
}