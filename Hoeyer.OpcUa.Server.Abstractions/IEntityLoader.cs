namespace Hoeyer.OpcUa.Server.Abstractions;

public interface IEntityLoader<T>
{
    public ValueTask<T> LoadCurrentState();
}