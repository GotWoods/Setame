using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace ConfigMan.ConfigurationProvider
{
    public class ConfigManConfigurationSource : IConfigurationSource
    {
        private readonly string _application;
        private readonly Uri _serviceUri;
        private readonly string _clientSecret;
        
        public ConfigManConfigurationSource(string application, Uri serviceUri, string clientSecret)
        {
            _application = application;
            _serviceUri = serviceUri;
            _clientSecret = clientSecret;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ConfigManConfigurationProvider(_application, _serviceUri, _clientSecret);
        }
    }
}