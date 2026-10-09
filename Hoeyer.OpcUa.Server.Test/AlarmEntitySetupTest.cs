using Hoeyer.OpcUa.Fixtures.Server.Entities;
using Hoeyer.OpcUa.Fixtures.Server.ServerFixture;
using Hoeyer.OpcUa.Server.Abstractions;
using Hoeyer.OpcUa.Server.Abstractions.NodeManagement;
using Opc.Ua;

namespace Hoeyer.OpcUa.Server.Test;

[ClassDataSource<RuntimeSelectedServer>(Shared = SharedType.PerTestSession)]
public sealed class AlarmEntitySetupTest(
    RuntimeSelectedServer fixture
)
{
    private readonly IEntityNodeManager manager = fixture.GetService<IEntityManagerHolder<AlarmTestEntity>>().Manager;
    private readonly IOpcEntityServer Server = fixture.GetService<IOpcEntityServer>();

    [Test]
    public async Task AlarmExample()
    {
        var context = Server.DefaultContext;
        BaseObjectState baseObject = manager.ManagedEntity.Select(node => node.BaseObject);
        var propertyState = manager.ManagedEntity.Select(node => node.PropertyStates.First());
        var alarm = manager.ManagedEntity.Select(node => node.AlarmsByProperty.First().Value);

        var alarmEnabled = false;
        alarm.OnEnableDisable = (context, node, enable) =>
        {
            alarmEnabled = enable;
            return ServiceResult.Good;
        };

        propertyState.Value = int.MaxValue;
        alarm.ClearChangeMasks(context, true);
        baseObject.ClearChangeMasks(context, true);

        await Assert.That(alarmEnabled).IsTrue();
    }
}