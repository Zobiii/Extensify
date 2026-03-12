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

        return new PluginLoadResult(plugins, errors);
    }

    private static void TryLoadAssembly(string assemblyPath, List<IPlugin> plugins, List<PluginLoadError> errors)
    {
        Assembly assembly;
        try
        {
            assembly = Assembly.LoadFrom(assemblyPath);
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
                if (Activator.CreateInstance(type) is IPlugin plugin)
                {
                    plugins.Add(plugin);
                }
            }
            catch (Exception ex)
            {
                var source = type.FullName ?? assemblyPath;
                errors.Add(new PluginLoadError(source, $"Plugin activation failed: {ex.Message}"));
            }
        }
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