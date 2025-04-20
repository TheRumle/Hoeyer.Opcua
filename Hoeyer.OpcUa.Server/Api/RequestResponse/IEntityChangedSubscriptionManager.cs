using Hoeyer.Common.Messaging;
using Hoeyer.OpcUa.Core.Entity.Node;

namespace Hoeyer.OpcUa.Server.Api.RequestResponse;

public interface IEntityChangedSubscriptionManager<T> : IMessagePublisher<IEntityNode>, ISubscriptionManager<T>;