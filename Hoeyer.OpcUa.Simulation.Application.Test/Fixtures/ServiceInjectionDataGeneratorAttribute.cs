using System.Diagnostics.CodeAnalysis;
using Hoeyer.Opc.Ua.Test.TUnit;

namespace Simulation.Application.Test.Fixtures;

[SuppressMessage(
    "Design", "S3993",
    Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
public sealed class ServiceInjectionDataGeneratorAttribute()
    : ServiceCollectionInjectionAttribute(new ServiceCollectionFixture().SimulationServices);