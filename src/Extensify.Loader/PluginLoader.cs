using System.Reflection;
using Extensify.Abstractions;

namespace Extensify.Loader;

/// <summary>
/// Discovers and instantiates plugins from assemblies in a directory.
/// </summary>
public sealed class PluginLoader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLoader"/> class.
    /// </summary>
    public PluginLoader()
    {
    }
    
    /// <summary>
    /// Scans a directory for plugin assemblies and loads valid plugins.
    /// </summary>
    /// <param name="pluginsDirectoryPath">The directory that contains plugin DLL files.</param>
    /// <returns>A load result with plugins and non-fatal errors.</returns>
    public PluginLoadResult LoadFromDirectory(string pluginsDirectoryPath)
    {
        var plugins = new List<IPlugin>();
        var errors = new List<PluginLoadError>();

        if (!Directory.Exists(pluginsDirectoryPath))
        {
            return new PluginLoadResult(plugins, errors);
        }

        foreach (var assemblyPath in Directory.EnumerateFiles(pluginsDirectoryPath, "*.dll", SearchOption.TopDirectoryOnly))    
        {
            TryLoadAssembly(assemblyPath, plugins, errors);
        }

        var uniquePlugins = RemoveDuplicateCommands(plugins, errors);
        return new PluginLoadResult(uniquePlugins, errors);
    }

    private static void TryLoadAssembly(string assemblyPath, List<IPlugin> plugins, List<PluginLoadError> errors)
    {
        Assembly assembly;
        try
        {
            var loadContext = new PluginLoadContext(assemblyPath);
            assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));
        }
        catch (Exception ex)
        {
            errors.Add(new PluginLoadError(assemblyPath, $"Assembly load failed: {ex.Message}"));
            return;
        }

        foreach (var type in GetPluginTypes(assembly, assemblyPath, errors))
        {
            try
            {
                if (Activator.CreateInstance(type) is not IPlugin plugin)
                {
                    errors.Add(new PluginLoadError(type.FullName ?? assemblyPath, "Activated type does not implement IPlugin"));
                    continue;
                }

                if (!TryValidatePlugin(plugin, type, errors))
                {
                    continue;
                }
                
                plugins.Add(plugin);
            }
            catch (Exception ex)
            {
                var source = type.FullName ?? assemblyPath;
                errors.Add(new PluginLoadError(source, $"Plugin activation failed: {ex.Message}"));
            }
        }
    }

    private static bool TryValidatePlugin(IPlugin plugin, Type pluginType, List<PluginLoadError> errors)
    {
        var source = pluginType.FullName ?? pluginType.Name;

        if (string.IsNullOrWhiteSpace(plugin.Name))
        {
            errors.Add(new PluginLoadError(source, "Plugin name is required."));
            return false;
        }

        if (string.IsNullOrWhiteSpace(plugin.Description))
        {
            errors.Add(new PluginLoadError(source, "Plugin description is required."));
            return false;
        }

        if (string.IsNullOrWhiteSpace(plugin.Command))
        {
            errors.Add(new PluginLoadError(source, "Plugin command is required."));
            return false;
        }

        return true;
    }

    private static IReadOnlyList<IPlugin> RemoveDuplicateCommands(List<IPlugin> plugins, List<PluginLoadError> errors)
    {
        var uniquePlugins = new List<IPlugin>();
        var seenCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var plugin in plugins)   
        {
            if (!seenCommands.Add(plugin.Command))
            {
                errors.Add(new PluginLoadError(plugin.Name, $"Duplicate command '{plugin.Command}' was ignored"));
                continue;
            }
            
            uniquePlugins.Add(plugin);
        }

        return uniquePlugins;
    }
    
    private static IEnumerable<Type> GetPluginTypes(Assembly assembly, string assemblyPath,
        List<PluginLoadError> errors)
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t is not null).Cast<Type>().ToArray();

            foreach (var loaderException in ex.LoaderExceptions.Where(e => e is not null))
            {
                errors.Add(new PluginLoadError(assemblyPath, $"Type discovery failed: {loaderException!.Message}"));
            }
        }
        catch (Exception ex)
        {
            errors.Add(new PluginLoadError(assemblyPath, $"Type discovery failed: {ex.Message}"));
            return Array.Empty<Type>();
        }

        return types.Where(type =>
            type.IsClass &&
            !type.IsAbstract &&
            typeof(IPlugin).IsAssignableFrom(type) &&
            type.GetConstructor(Type.EmptyTypes) is not null);
    }
}