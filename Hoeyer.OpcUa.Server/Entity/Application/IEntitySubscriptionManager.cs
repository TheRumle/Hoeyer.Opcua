using FluentResults;
using Opc.Ua.Server;
using Subscription = Opc.Ua.Client.Subscription;

namespace Hoeyer.OpcUa.Server.Entity.Application;

public interface IEntitySubscriptionManager : ISubscriptionManager
{
    public Result Subscribe(Subscription subscription);
    public Result Unsubscribe(Subscription subscription);
    
}