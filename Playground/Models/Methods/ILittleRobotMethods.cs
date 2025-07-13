using Hoeyer.OpcUa.Core;

namespace Playground.Models.Methods;

[OpcUaEntityMethods<MyLittleRobot>]
public interface ILittleRobotMethods
{
    Task MoveToPosition(Position position);
    Task IncrementSpeed();
}