using System;
using System.Collections.Generic;
using System.Linq;
using Hoeyer.Opc.Ua.Test.TUnit.Extensions;
using Hoeyer.OpcUa.Entity.CompileTime.Testing.EntityDefinitions;

namespace Hoeyer.OpcUa.Entity.CompileTime.Testing.Generators;

public sealed class EntityServiceInterfaceGeneratorAttribute : DataSourceGeneratorAttribute<ServiceInterfaceSourceCode>
{
    /// <inheritdoc />
    public override IEnumerable<Func<ServiceInterfaceSourceCode>> GenerateDataSources(
        DataGeneratorMetadata dataGeneratorMetadata)
        => TestEntities.Valid.SelectMany(TestBehaviours.GetServiceInterfaceSourceCodeFor).SelectFunc();
}