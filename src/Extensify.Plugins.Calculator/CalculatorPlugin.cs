using System.Globalization;
using Extensify.Abstractions;

namespace Extensify.Plugins.Calculator;

/// <summary>
/// Provides basic calculator operations for the Extensify host.
/// </summary>
public sealed class CalculatorPlugin : IPlugin
{
    /// <summary>
    /// Gets the display name of the plugin.
    /// </summary>
    public string Name => "calculator";

    /// <summary>
    /// Gets the short description of the plugin.
    /// </summary>
    public string Description => "Basic math operations";

    /// <summary>
    /// Gets the command key used by the host.
    /// </summary>
    public string Command => "calculator";

    /// <summary>
    /// Executes one calculator operation with two numeric operands.
    /// </summary>
    /// <param name="args">The operation and operands in the format &lt;op&gt; &lt;x&gt; &lt;y&gt;.</param>
    /// <returns>The execution result containing either output or an error message.</returns>
    public PluginExecutionResult Execute(string[] args)
    {
        if (args.Length != 3)
        {
            return PluginExecutionResult.Failure("Usage: calculator <add|sub|mul|div> <x> <y>");
        }

        if (!double.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var left) ||
            !double.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var right))
        {
            return PluginExecutionResult.Failure("Both numbers must be valid numeric values.");
        }

        var operations = args[0].ToLowerInvariant();

        return operations switch
        {
            "add" => PluginExecutionResult.Success((left + right).ToString(CultureInfo.InvariantCulture)),
            "sub" => PluginExecutionResult.Success((left - right).ToString(CultureInfo.InvariantCulture)),
            "mul" => PluginExecutionResult.Success((left * right).ToString(CultureInfo.InvariantCulture)),
            "div" when right == 0 => PluginExecutionResult.Failure("Division by zero is not allowed."),
            "div" => PluginExecutionResult.Success((left / right).ToString(CultureInfo.InvariantCulture)),
            _ => PluginExecutionResult.Failure("Unknown operation. Use: add, sub, mul, div.")
        };
    }
}
