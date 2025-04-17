using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Api.RequestResponse;

public interface IEntityChangedMessenger<out T> : IMessagePublisher<IEntityNode>, IMessageSubscribable<T>;