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
    private readonly IEntityNodeManager _manager = fixture.GetService<IEntityManagerHolder<AlarmTestEntity>>().Manager!;
    private readonly IOpcEntityServer _server = fixture.GetService<IOpcEntityServer>();

    [Test]
    [Skip("Postponed to update opcua foundation")]
    public async Task AlarmExample()
    {
        var context = _server.DefaultContext;
        var baseObject = _manager.ManagedEntity.Select(node => node.BaseObject);
        PropertyState<int> propertyWithAlarm = GetPropertyState();
        LimitAlarmState alarm = GetAlarm();

        var alarmIsCalled = false;
        alarm.OnEnableDisable = (context, node, enable) =>
        {
            alarmIsCalled = true;
            return ServiceResult.Good;
        };

        propertyWithAlarm.Value = int.MaxValue;
        alarm.ClearChangeMasks(context, true);
        baseObject.ClearChangeMasks(context, true);

        await Assert.That(alarmIsCalled).IsTrue();
    }

    private LimitAlarmState GetAlarm()
    {
        return _manager.ManagedEntity.Select(node => node.AlarmsByProperty.First().Value);
    }

    private PropertyState<int> GetPropertyState()
    {
        return _manager.ManagedEntity.Select(node => node.PropertyStates.OfType<PropertyState<int>>().First());
    }
}