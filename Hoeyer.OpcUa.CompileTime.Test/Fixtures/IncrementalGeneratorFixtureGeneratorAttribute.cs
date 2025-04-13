using System.Diagnostics.CodeAnalysis;
using Hoeyer.OpcUa.Server.SourceGeneration.Generation;
using Microsoft.CodeAnalysis;

namespace Hoeyer.OpcUa.CompileTime.Test.Fixtures;

[SuppressMessage("Design", "S3993", Justification = "TUnits' attributeusage must not and cannot be overwritten.")]
internal sealed class
    IncrementalGeneratorFixtureGeneratorAttribute : TypesWithEmptyCtorScanningGeneratorAttribute<IIncrementalGenerator, EntityNodeStructureFactoryGenerator>;