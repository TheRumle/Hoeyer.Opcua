using System.Diagnostics.CodeAnalysis;
using Opc.Ua;

namespace Hoeyer.OpcUa.Client.Abstractions.Calling;

[SuppressMessage("Maintainability", checkId: "S2326", Justification = "Used for injection and modelling")]
public interface IMethodCaller<TEntity>
{
    public Task CallMethod(string methodName, CancellationToken token = default, params Variant[] args);
    public Task<T> CallMethod<T>(string methodName, CancellationToken token = default, params Variant[] args);
}