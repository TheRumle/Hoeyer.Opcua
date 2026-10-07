using Hoeyer.OpcUa.Core.Abstractions;
using Hoeyer.OpcUa.Fixtures.Common.Utils;
using Hoeyer.OpcUa.Fixtures.Server.Entities;

namespace Hoeyer.OpcUa.Server.Test;

[ServiceInjection]
public sealed class AlarmEntitySetupTest(
    NonOwned<IEntityNodeStructureFactory<AlarmTestEntity>> entityFactory
)
{
}