using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Entity;
using Hoeyer.OpcUa.Core.Entity.Node;
using Hoeyer.OpcUa.Server.Api.RequestResponse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hoeyer.OpcUa.Server.Entity.Application;

[OpcUaEntityService(typeof(IEntityChangedMessenger<>), ServiceLifetime.Singleton)]
public sealed class EntityChangedMessenger<T>(IEntityTranslator<T> translator, ILogger? logger = null) 
    : MessagePublisher<T>(logger), IEntityChangedMessenger<T>
{
    public void Publish(IEntityNode message) => Publish(translator.Translate(message));
}


