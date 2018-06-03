using Microsoft.Extensions.Configuration;

namespace ApprovalFlowSample
{
    public class Settings
    {
        public static Settings Instance { get; } = new Settings();

        private readonly IConfigurationRoot _configuration;

        private Settings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public string ConsumerKey => _configuration[nameof(ConsumerKey)];
        public string ConsumerSecret => _configuration[nameof(ConsumerSecret)];
        public string AccessToken => _configuration[nameof(AccessToken)];
        public string AccessSecret => _configuration[nameof(AccessSecret)];
    }
}
