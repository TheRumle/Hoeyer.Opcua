using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration.Health;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class HealthCheck : IServerStartedHealthCheck
{
    private readonly TaskCompletionSource<bool> _tcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public bool IsServerStarted => _tcs.Task.IsCompletedSuccessfully;

    public Task<bool> ServerRunning() => _tcs.Task;

    public void MarkCompleted()
        => _tcs.TrySetResult(true);

    public void MarkFailed(Exception exception)
        => _tcs.TrySetException(exception);
}