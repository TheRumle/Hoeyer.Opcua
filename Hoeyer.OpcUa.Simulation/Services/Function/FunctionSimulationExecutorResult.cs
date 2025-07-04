using System;
using System.Collections.Generic;

namespace Hoeyer.OpcUa.Server.Simulation.Services.Function;

internal record struct FunctionSimulationExecutorResult<TEntity, TResult>(
    IEnumerable<(TEntity Previous, DateTime Time, TEntity Reached)> Transitions,
    TResult ReturnValue);