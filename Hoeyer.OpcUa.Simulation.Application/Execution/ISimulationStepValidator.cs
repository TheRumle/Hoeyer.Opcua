using System.Collections.Generic;
using Hoeyer.OpcUa.Simulation.Api.Execution.ExecutionSteps;

namespace Hoeyer.OpcUa.Simulation.Execution;

public interface ISimulationStepValidator
{
    void ValidateOrThrow(IEnumerable<ISimulationStep> steps);
}