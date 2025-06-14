﻿using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;
using Hoeyer.opcUa.TestEntities;
using JetBrains.Annotations;
using Opc.Ua;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(ReflectionBasedEntityStructureFactory<>))]
[TestSubject(typeof(IEntityNodeStructureFactory<>))]
public sealed class ReflectionBasedEntityStructureFactoryTest
{
    private readonly Type _entityType = typeof(AllPropertyTypesEntity);
    private readonly ReflectionBasedEntityStructureFactory<AllPropertyTypesEntity> _testSubject = new();

    [Test]
    public async Task BaseObjectNameMatchesTypeName()
    {
        BaseObjectState result = _testSubject.Create(2).BaseObject;
        await Assert.That(result.BrowseName.Name).IsEqualTo(nameof(AllPropertyTypesEntity));
    }

    [Test]
    public async Task PropertyNamesMatch()
    {
        Dictionary<string, PropertyState>.KeyCollection nodePropertyNames =
            _testSubject.Create(2).PropertyByBrowseName.Keys;

        HashSet<string> subjectPropertyNames = _entityType.GetProperties().Select(e => e.Name).ToHashSet();
        using (Assert.Multiple())
        {
            foreach (var name in nodePropertyNames)
            {
                await Assert.That(subjectPropertyNames).Contains(name);
            }
        }
    }
}