using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Generators;

public sealed class ValidEntitySourceCodeGeneratorAttribute : DataSourceGeneratorAttribute<EntitySourceCode>
{
    /// <inheritdoc />
    public override IEnumerable<Func<EntitySourceCode>> GenerateDataSources(DataGeneratorMetadata dataGeneratorMetadata)
    {
        return TestEntities.Valid.Select(source => (Func<EntitySourceCode>)(() => source));
    }
}

public sealed class EntityServiceInterfaceGeneratorAttribute : DataSourceGeneratorAttribute<ServiceInterfaceSourceCode>
{
    /// <inheritdoc />
    public override IEnumerable<Func<ServiceInterfaceSourceCode>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
        => TestEntities.Valid.SelectMany(TestBehaviours.GetServiceInterfaceSourceCodeFor).SelectFunc();
}