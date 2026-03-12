using Extensify.Abstractions;

namespace Extensify.Plugins.WeatherMock;

public sealed class WeatherMockPlugin : IPlugin
{
    public string Name => "weathermock";

    public string Description => "Mock weather output";

    public string Command => "weathermock";

    public PluginExecutionResult Execute(string[] args)
    {
        var city = args.Length > 0 ? string.Join(' ', args) : "Vienna";
        var condition = city.ToLowerInvariant() switch
        {
            "vienna" => "Cloudy, 14°C",
            "berlin" => "Light rain, 11°C",
            "london" => "Windy, 9°C",
            _ => "Sunny, 18°C"
        };

        return PluginExecutionResult.Success($"Weather in {city}: {condition}");
    }
}