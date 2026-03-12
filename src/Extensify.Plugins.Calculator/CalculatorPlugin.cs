using System.Globalization;
using Extensify.Abstractions;

namespace Extensify.Plugins.Calculator;

public sealed class CalculatorPlugin : IPlugin
{
    public string Name => "calculator";

    public string Description => "Basic math operations";

    public string Command => "calculator";

    public PluginExecutionResult Execute(string[] args)
    {
        if (args.Length != 3)
        {
            return PluginExecutionResult.Failure("Usage: calculator <add|sub|mul|div> <x> <y>");
        }

        if (!double.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var left) ||
            !double.TryParse(args[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var right))
        {
            return PluginExecutionResult.Failure("Both numbers must be a valid numeric values.");
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