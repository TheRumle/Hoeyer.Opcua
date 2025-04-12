namespace Hoeyer.Common.Messaging;

public interface IMessageSubscriber<in TState>
{
    public void OnMessagePublished(TState stateChange);
}