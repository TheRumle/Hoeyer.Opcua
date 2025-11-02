using System.Threading.Tasks;

namespace Hoeyer.OpcUa.Client.Api.Monitoring;

/// <summary>
///     An observer that observe any state change of the given entity.
/// </summary>
public interface IStateChangeObserver<T>
{
    Task<ISubscribedStateChangeMonitor<T>> BeginObserveAsync();
}