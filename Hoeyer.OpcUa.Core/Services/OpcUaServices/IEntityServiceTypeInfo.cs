using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services.OpcUaServices;

public interface IEntityServiceTypeInfo
{
    Type ImplementationType { get; }
    ServiceLifetime ServiceLifetime { get; }
}