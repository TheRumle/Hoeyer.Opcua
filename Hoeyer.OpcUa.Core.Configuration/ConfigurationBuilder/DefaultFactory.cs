namespace Hoeyer.OpcUa.Core.Configuration.ConfigurationBuilder;

public sealed class DefaultFactory(Func<IApplicationConfigurationRequirements> requirements)
    : IApplicationRequirementsFactory
{
    public IApplicationConfigurationRequirements Get() => requirements.Invoke();
}