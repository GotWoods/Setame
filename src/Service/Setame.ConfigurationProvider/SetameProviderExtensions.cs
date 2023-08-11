using Microsoft.Extensions.Configuration;

namespace Setame.ConfigurationProvider;

public static class SetameProviderExtensions
{
    public static IConfigurationBuilder AddSetame(this IConfigurationBuilder builder, string application, string environment, Uri serviceLocation, string clientToken)
    {
        builder.Add(new SetameConfigurationSource(application, serviceLocation, environment, clientToken));
        return builder;
    }
}