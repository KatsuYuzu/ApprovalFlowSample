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

        public string TwitterConsumerKey => _configuration[nameof(TwitterConsumerKey)];
        public string TwitterConsumerSecret => _configuration[nameof(TwitterConsumerSecret)];
        public string TwitterAccessToken => _configuration[nameof(TwitterAccessToken)];
        public string TwitterAccessSecret => _configuration[nameof(TwitterAccessSecret)];
    }
}
