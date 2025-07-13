namespace Hoeyer.OpcUa.Simulation.Verification;

public record struct VerificationError<TEntity>(
    string Description,
    string IndexOfError,
    List<TraceStep<TEntity>> Violators);