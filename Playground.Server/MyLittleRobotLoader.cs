using Hoeyer.OpcUa.Server.Api;
using Playground.Modelling.Models;

namespace Playground.Server;

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