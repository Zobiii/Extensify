using Extensify.Loader;

var pluginsDirectoryPath = Path.Combine(AppContext.BaseDirectory, "plugins");
Directory.CreateDirectory(pluginsDirectoryPath);

var loader = new PluginLoader();
var loadResult = loader.LoadFromDirectory(pluginsDirectoryPath);

var pluginsByCommand = loadResult.Plugins
    .GroupBy(plugin => plugin.Command, StringComparer.OrdinalIgnoreCase)
    .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

Console.WriteLine("Extensify host starting...");
Console.WriteLine($"{pluginsByCommand.Count} plugins loaded");
Console.WriteLine("Type: plugins | run <command> [args] | exit");

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

    Console.WriteLine("Unknown command. Use: plugins | run <command> [args] | exit");
}