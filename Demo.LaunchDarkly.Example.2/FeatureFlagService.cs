using LaunchDarkly.Client;
using Microsoft.Extensions.Options;

namespace Demo.LaunchDarkly.Example._2
{
    public  class FeatureFlagService : IFeatureFlagsService
    {
        private readonly LaunchDarklyOptions _launchDarklyOptions;
        private readonly ILdClient _ldClient;

        public FeatureFlagService(IOptions<LaunchDarklyOptions> options, ILdClient ldClient)
        {
            // _ldClient = new LdClient(configuration["LaunchDarkly:SdkKey"]);
            _launchDarklyOptions = options.Value;
            _ldClient = ldClient;
        }

        public T GetFeatureFlag<T>(string featureFlagKey, string userKey)
        { 
            var user = User.WithKey(userKey);
            var type = typeof(T);

            return type switch
            {
                _ when type == typeof(bool) => (T)(object)_ldClient.BoolVariation(featureFlagKey, user, default),
                _ when type == typeof(int) => (T)(object)_ldClient.IntVariation(featureFlagKey, user, default),
                _ when type == typeof(float) => (T)(object)_ldClient.FloatVariation(featureFlagKey, user, default),
                _ when type == typeof(string) => (T)(object)_ldClient.StringVariation(featureFlagKey, user, default),
                _ => default
            };
        }
    }
}
