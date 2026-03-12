using Extensify.Abstractions;
using Extensify.Loader;

var pluginsDirectoryPath = Path.Combine(AppContext.BaseDirectory, "plugins");
Directory.CreateDirectory(pluginsDirectoryPath);

var loader = new PluginLoader();
var loadResult = loader.LoadFromDirectory(pluginsDirectoryPath);

var pluginsByCommand = loadResult.Plugins
    .GroupBy(plugin => plugin.Command, StringComparer.OrdinalIgnoreCase)
    .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

Console.WriteLine("Extensify host starting...");

if (loadResult.Errors.Count > 0)
{
    Console.WriteLine("Plugin load warnings:");
    foreach (var error in loadResult.Errors) 
    {
        Console.WriteLine($"- {error.Source}: {error.Message}");
    }
}

Console.WriteLine($"{pluginsByCommand.Count} plugins loaded");
Console.WriteLine("Type: plugins | help <command> | run <command> [args] | exit");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var action = parts[0];

    if (action.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (action.Equals("plugins", StringComparison.OrdinalIgnoreCase))
    {
        Console.WriteLine("Loaded plugins:");
        foreach (var plugin in pluginsByCommand.Values.OrderBy(plugin => plugin.Command))
        {
            Console.WriteLine($"- {plugin.Command,-12} {plugin.Description}");
        }

        continue;
    }

    if (action.Equals("help", StringComparison.OrdinalIgnoreCase))
    {
        if (parts.Length == 1)
        {
            PrintGeneralHelp(pluginsByCommand);
        }
        else
        {
            PrintPluginHelp(parts[1], pluginsByCommand);
        }
        
        continue;
    }

    if (action.Equals("run", StringComparison.OrdinalIgnoreCase))
    {
        if (parts.Length < 2)
        {
            Console.WriteLine("Usage: run <command> [args]");
            continue;
        }

        var command = parts[1];
        if (!pluginsByCommand.TryGetValue(command, out var plugin))
        {
            Console.WriteLine($"Plugin '{command}' not found.");
            continue;
        }

        var pluginArgs = parts.Skip(2).ToArray();
        var result = plugin.Execute(pluginArgs);

        Console.WriteLine(result.IsSuccess
            ? $"Result: {result.Message}"
            : $"Error: {result.Message}");

        continue;
    }

    Console.WriteLine("Unknown command. Use: plugins | help <command> | run <command> [args] | exit");
}

static void PrintGeneralHelp(IReadOnlyDictionary<string, IPlugin> pluginsByCommand)
{
    Console.WriteLine("Host commands:");
    Console.WriteLine("- plugins");
    Console.WriteLine("- help [command]");
    Console.WriteLine("- run <command> [args]");
    Console.WriteLine("- exit");
    Console.WriteLine("Plugin commands:");

    foreach (var plugin in pluginsByCommand.Values.OrderBy(plugin => plugin.Command))
    {
        Console.WriteLine($"- {plugin.Command,-12} {plugin.Description}");
    }
}

static void PrintPluginHelp(string command, IReadOnlyDictionary<string, IPlugin> pluginsByCommand)
{
    if (!pluginsByCommand.TryGetValue(command, out var plugin))
    {
        Console.WriteLine($"Plugin '{command}' not found.");
        return;
    }

    Console.WriteLine($"Command: {plugin.Command}");
    Console.WriteLine($"Name: {plugin.Name}");
    Console.WriteLine($"Description: {plugin.Description}");
    Console.WriteLine($"Usage: run {plugin.Command} [args]");
}
