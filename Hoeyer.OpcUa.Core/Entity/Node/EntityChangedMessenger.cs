using Hoeyer.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Core.Entity.Node;

[OpcUaEntityService(typeof(IEntityChangedMessenger<>), ServiceLifetime.Singleton)]
public sealed class EntityChangedMessenger<T>(IEntityTranslator<T> translator, ILogger? logger = null) 
    : MessagePublisher<T>(logger), IEntityChangedMessenger<T>
{
    public void Publish(IEntityNode message) => Publish(translator.Translate(message));
}

public interface IEntityChangedMessenger<out T> : IMessagePublisher<IEntityNode>, ISubscribable<T>;
