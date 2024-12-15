using System;
using System.Threading.Tasks;
using FluentResults;

namespace Hoeyer.Common.Extensions.Functional;

public static class TaskExtensions 
{
    public static async Task<Result<U>> Bind<T, U>(
        this Task<Result<T>> task, Func<T, Result<U>> f) 
        => (await task).Bind(f);

    public static async Task<Result<U>> Bind<T, U>(
        this Task<Result<T>> task, Func<T, Task<Result<U>>> f) 
        => await (await task).Bind(f);
}