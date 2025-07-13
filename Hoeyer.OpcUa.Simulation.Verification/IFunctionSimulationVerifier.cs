namespace Hoeyer.OpcUa.Simulation.Verification;

public interface IFunctionSimulationVerifier<TEntity, in TArgs, in TReturn> : ISimulationVerifier<TEntity, TArgs>
{
    public ValueTask<IEnumerable<VerificationError<TEntity>>> VerifyReturn(TArgs input, TReturn output);
}