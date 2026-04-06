using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hoeyer.Common.Utilities.Threading;

public static class LockingExtensions
{
    public static async Task<IAsyncDisposable> LockAsync(this SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        return new AsyncReleaser(semaphore);
    }

    private class AsyncReleaser : IAsyncDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public AsyncReleaser(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public ValueTask DisposeAsync()
        {
            _semaphore.Release();
            return new ValueTask(Task.CompletedTask);
        }
    }
}