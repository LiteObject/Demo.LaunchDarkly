namespace Demo.LaunchDarkly.Example._2
{
    public interface IFeatureFlagsService
    {
        T GetFeatureFlag<T>(string featureFlagKey, string userKey);
    }
}
