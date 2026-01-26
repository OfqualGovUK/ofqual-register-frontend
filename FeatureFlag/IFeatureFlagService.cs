namespace Ofqual.Common.RegisterFrontend.FeatureFlag;

public interface IFeatureFlagService
{
    public bool IsFeatureEnabled(string featureName);
}
