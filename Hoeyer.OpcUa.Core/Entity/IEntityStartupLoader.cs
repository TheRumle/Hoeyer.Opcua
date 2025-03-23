namespace Hoeyer.OpcUa.Core.Entity;

public interface IEntityStartupLoader<out T> where T : new()
{
    public T LoadStartUpState();
}