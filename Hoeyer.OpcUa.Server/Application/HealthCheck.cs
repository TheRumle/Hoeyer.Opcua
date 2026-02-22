using System;
using System.Threading.Tasks;
using Hoeyer.OpcUa.Core.Configuration.ServerTarget;

namespace Hoeyer.OpcUa.Server.Application;

public sealed class HealthCheck : IServerStartedHealthCheck
{
    private readonly TaskCompletionSource<object?> _tcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public bool IsServerStarted => _tcs.Task.IsCompletedSuccessfully;

    public Task ServerRunning() => _tcs.Task;

    public void MarkCompleted()
        => _tcs.TrySetResult(null);

    public void MarkFailed(Exception exception)
        => _tcs.TrySetException(exception);
}