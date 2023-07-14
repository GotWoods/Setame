using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigMan.ConfigurationProvider;

public class TrackingConfiguration : IConfiguration
{
    private readonly IConfiguration _configuration;

    public TrackingConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string this[string key]
    {
        get
        {
            // Implement the tracking logic here
            Console.WriteLine($"Accessed configuration value for key: {key}");

            return _configuration[key];
        }
        set => _configuration[key] = value;
    }

    public IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return _configuration.GetChildren();
    }

    public IChangeToken GetReloadToken()
    {
        return _configuration.GetReloadToken();
    }

    // public void Reload()
    // {
    //     _configuration.Reload();
    // }
}