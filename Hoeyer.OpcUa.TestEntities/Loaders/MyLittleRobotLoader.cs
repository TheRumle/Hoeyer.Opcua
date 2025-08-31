using Hoeyer.OpcUa.Server.Api;
using Hoeyer.OpcUa.TestEntities.Models;

namespace Hoeyer.OpcUa.TestEntities.Loaders;

public sealed class MyLittleRobotLoader : IEntityLoader<MyLittleRobot>
{
    public ValueTask<MyLittleRobot> LoadCurrentState()
    {
        return new ValueTask<MyLittleRobot>(new MyLittleRobot
        {
            Name = "Robotto Jr.",
            TargetPosition = Position.TheSecretUndergroundLab,
            Speed = 2.0f,
            FirmwareVersion = "2.0.132"
        });
    }
}