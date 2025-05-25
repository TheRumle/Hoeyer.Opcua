using Hoeyer.OpcUa.Core;
using Hoeyer.OpcUa.Core.Api;
using Hoeyer.OpcUa.Core.Application.Translator;

namespace Playground;

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

[OpcUaEntityService(typeof(IEntityTranslator<>))]
public sealed class GantryTranslator : IEntityTranslator<Gantry>
{
    public void AssignToNode(Gantry state, IEntityNode node)
    {
        node.PropertyByBrowseName["Position"].Value = state.Position;
        node.PropertyByBrowseName["Moving"].Value = state.Moving;
        node.PropertyByBrowseName["Speeds"].Value = state.Speeds.ToArray();
        node.PropertyByBrowseName["CurrentId"].Value = state.CurrentId;
        node.PropertyByBrowseName["message"].Value = state.message;
        node.PropertyByBrowseName["messages"].Value = state.messages.ToArray();
        node.PropertyByBrowseName["Names"].Value = state.Names.ToArray();
    }

    public Gantry Translate(IEntityNode state)
    {
        var Position = DataTypeToTranslator.TranslateToSingle<Position>(state, "Position");
        var Moving = DataTypeToTranslator.TranslateToSingle<bool>(state, "Moving");
        var CurrentId = DataTypeToTranslator.TranslateToSingle<Guid>(state, "CurrentId");
        var message = DataTypeToTranslator.TranslateToSingle<string>(state, "message");
        List<int> Speeds = DataTypeToTranslator.TranslateToCollection<List<int>, int>(state, "Speeds");
        List<string> messages = DataTypeToTranslator.TranslateToCollection<List<string>, string>(state, "messages");
        List<string> Names = DataTypeToTranslator.TranslateToCollection<List<string>, string>(state, "Names");
        return new Gantry
        {
            Position = Position, Moving = Moving, Speeds = Speeds, CurrentId = CurrentId, message = message,
            messages = messages, Names = Names
        };
    }
}