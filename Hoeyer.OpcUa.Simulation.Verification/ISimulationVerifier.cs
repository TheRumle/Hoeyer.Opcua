namespace Hoeyer.OpcUa.Simulation.Verification;

public interface ISimulationVerifier<TEntity, in TArgs>
{
    public ValueTask<IEnumerable<VerificationError<TEntity>>> VerifyTrace(TArgs input, List<TraceStep<TEntity>> trace);
}