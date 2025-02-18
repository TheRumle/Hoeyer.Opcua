using System.Collections.Generic;

namespace Hoeyer.OpcUa.Configuration.EntityServerBuilder;

public interface IEndpointsStep
{
    IEntityServerConfigurationBuildable WithEndpoints(List<string> endpoints);
}