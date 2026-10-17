namespace Hoeyer.OpcUa.Client.Abstractions.Writing;

public interface IEntityWriter;

public interface IEntityWriter<in T> : IEntityWriter
{
    public Task AssignEntityValues(T entity, CancellationToken cancellationToken = default);
}