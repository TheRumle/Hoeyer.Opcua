using System.Threading.Tasks;
using Hoeyer.OpcUa.Client.Abstractions.Connection;

namespace Hoeyer.OpcUa.Client.Application.Subscriptions;

internal interface ISubscriptionTransferStrategy
{
    public Task TransferSubscriptionsBetween(IEntitySession oldSession, IEntitySession newSession);
}