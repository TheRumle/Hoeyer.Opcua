using System.Collections.Generic;

namespace Hoeyer.OpcUa.Server.ServiceConfiguration.Builder;

public interface IEndpointsStep
{
    IEntityServerConfigurationBuildable WithEndpoints(List<string> endpoints);
}