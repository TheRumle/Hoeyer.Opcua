using System.Collections.Generic;

namespace Hoeyer.OpcUa.Core.Configuration.EntityServerBuilder;

public interface IEndpointsStep : IEntityServerConfigurationBuildable
{
    /// <summary>
    /// </summary>
    /// <param name="endpoints">
    ///     <example>["localhost"]</example>
    /// </param>
    /// <returns></returns>
    IEntityServerConfigurationBuildable WithEndpoints(List<string> endpoints);
}