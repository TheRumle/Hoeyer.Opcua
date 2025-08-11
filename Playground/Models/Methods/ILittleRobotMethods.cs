using Hoeyer.OpcUa.Core;

namespace Playground.Models.Methods;

[OpcUaAgentMethods<MyLittleRobot>]
public interface ILittleRobotMethods
{
    Task MoveToPosition(Position position);
    Task IncrementSpeed();
}