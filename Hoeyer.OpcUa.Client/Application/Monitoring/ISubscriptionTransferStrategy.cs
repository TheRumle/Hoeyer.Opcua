using Hoeyer.OpcUa.Client.Abstractions.Connection;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

public interface ISubscriptionTransferStrategy
{
    public Task TransferSubscriptionsBetween(IEntitySession oldSession, IEntitySession newSession);
}