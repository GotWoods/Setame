using Microsoft.Extensions.Configuration;

namespace Setame.ConfigurationProvider
{
    public class SetameConfigurationSource : IConfigurationSource
    {
        private readonly string _application;
        private readonly Uri _serviceUri;
        private readonly string _environment;
        private readonly string _clientToken;
        
        public SetameConfigurationSource(string application, Uri serviceUri, string environment, string clientToken)
        {
            _application = application;
            _serviceUri = serviceUri;
            _environment = environment;
            _clientToken = clientToken;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SetameConfigurationProvider(_application, _serviceUri, _environment, _clientToken);
        }
    }
}