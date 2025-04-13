using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Server.Entity.Observability;

public interface IEntityChangedMessenger<T> : IMessagePublisher<T>, IEntityChangedMessenger;

[OpcUaEntityService(typeof(IEntityChangedMessenger<>))]
public sealed class EntityChangedMessenger<T>(IEntityTranslator<T> translator, ILogger? logger = null) :  MessagePublisher<T>(logger), IEntityChangedMessenger<T>
{
    public void Publish(IEntityNode node) => Publish(translator.Translate(node));
}