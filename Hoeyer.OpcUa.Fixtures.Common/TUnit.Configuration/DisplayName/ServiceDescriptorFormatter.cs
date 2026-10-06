using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Fixtures.Common.TUnit.Configuration.DisplayName;

public sealed class ServiceDescriptorFormatter : ArgumentDisplayFormatter
{
    public override bool CanHandle(object? value)
        => value is ServiceDescriptor;

    public override string FormatValue(object? value)
    {
        var descriptor = (ServiceDescriptor)value!;

        return descriptor.ServiceType.Name;
    }
}