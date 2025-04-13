using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Server.Entity.Observability;

[OpcUaEntityService(typeof(IEntityChangeMessengerFactory))]
public sealed class EntityChangeMessengerFactory<T>(IEntityTranslator<T> translator) : IEntityChangeMessengerFactory
{
    public IEntityChangedMessenger Create(ILogger logger) => new EntityChangedMessenger<T>(translator, logger);
}

public interface IEntityChangeMessengerFactory
{
    public IEntityChangedMessenger Create(ILogger logger);
}