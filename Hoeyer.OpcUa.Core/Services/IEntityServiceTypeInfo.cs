using System;
using Microsoft.Extensions.DependencyInjection;

namespace Hoeyer.OpcUa.Core.Services;

public interface IEntityServiceTypeInfo
{
    Type ImplementationType { get; }
    ServiceLifetime ServiceLifetime { get; }
}