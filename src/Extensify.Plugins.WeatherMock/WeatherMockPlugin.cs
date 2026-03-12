using Extensify.Abstractions;

namespace Extensify.Plugins.WeatherMock;

/// <summary>
/// Provides mock weather output for demonstration and testing.
/// </summary>
public sealed class WeatherMockPlugin : IPlugin
{
    /// <summary>
    /// Gets the display name of the plugin.
    /// </summary>
    public string Name => "weathermock";

    /// <summary>
    /// Gets the short description of the plugin.
    /// </summary>
    public string Description => "Mock weather output";

    /// <summary>
    /// Gets the command key used by the host.
    /// </summary>
    public string Command => "weathermock";

    /// <summary>
    /// Returns mock weather data for a requested city.
    /// </summary>
    /// <param name="args">Optional city name tokens.</param>
    /// <returns>The execution result containing the mock weather text.</returns>
    public PluginExecutionResult Execute(string[] args)
    {
        var city = args.Length > 0 ? string.Join(' ', args) : "Vienna";
        var condition = city.ToLowerInvariant() switch
        {
            "vienna" => "Cloudy, 14 C",
            "berlin" => "Light rain, 11 C",
            "london" => "Windy, 9 C",
            _ => "Sunny, 18 C"
        };

        return PluginExecutionResult.Success($"Weather in {city}: {condition}");
    }
}
