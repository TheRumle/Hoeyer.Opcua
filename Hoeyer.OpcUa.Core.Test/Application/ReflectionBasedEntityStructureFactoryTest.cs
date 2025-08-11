using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.NodeStructureFactory;
using Hoeyer.OpcUa.TestEntities;
using JetBrains.Annotations;

namespace Hoeyer.OpcUa.Core.Test.Application;

[TestSubject(typeof(ReflectionBasedAgentStructureFactory<>))]
[TestSubject(typeof(IAgentStructureFactory<>))]
public sealed class ReflectionBasedAgentStructureFactoryTest
{
    private readonly Type _agentType = typeof(AllPropertyTypesAgent);
    private readonly ReflectionBasedAgentStructureFactory<AllPropertyTypesAgent> _testSubject = new();

    [Test]
    public async Task BaseObjectNameMatchesTypeName()
    {
        var result = _testSubject.Create(2).BaseObject;
        await Assert.That(result.BrowseName.Name).IsEqualTo(nameof(AllPropertyTypesAgent));
    }

    [Test]
    public async Task PropertyNamesMatch()
    {
        var nodePropertyNames = _testSubject.Create(2).PropertyByBrowseName.Keys;

        var subjectPropertyNames = _agentType.GetProperties().Select(e => e.Name).ToHashSet();
        using (Assert.Multiple())
        {
            foreach (var name in nodePropertyNames)
            {
                await Assert.That(subjectPropertyNames).Contains(name);
            }
        }
    }
}