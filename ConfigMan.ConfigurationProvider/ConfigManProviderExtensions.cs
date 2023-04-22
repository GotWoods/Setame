using ConfigMan.ConfigurationProvider;
using Microsoft.Extensions.Configuration;

public static class ConfigManProviderExtensions
{
    public static IConfigurationBuilder AddConfigMan(this IConfigurationBuilder builder, string application, Uri serviceLocation, string clientSecret)
    {
        builder.Add(new ConfigManConfigurationSource(application, serviceLocation, clientSecret));
        return builder;
    }
}