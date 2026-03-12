using System.Reflection;
using System.Runtime.Loader;
using Extensify.Abstractions;

namespace Extensify.Loader;

/// <summary>
/// Loads a plugin assembly and its dependencies in an isolated context.
/// </summary>
internal sealed class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLoadContext"/> class.
    /// </summary>
    /// <param name="pluginAssemblyPath">The full path to the plugin assembly.</param>
    public PluginLoadContext(string pluginAssemblyPath) : base(isCollectible: true)
    {
        _resolver = new AssemblyDependencyResolver(pluginAssemblyPath);
    }
    
    /// <inheritdoc />
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (string.Equals(assemblyName.Name, typeof(IPlugin).Assembly.GetName().Name, StringComparison.Ordinal))
        {
            return typeof(IPlugin).Assembly;
        }

        var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath is null)
        {
            return null;
        }

        return LoadFromAssemblyPath(assemblyPath);
    }

    /// <inheritdoc />
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath is null)
        {
            return IntPtr.Zero;
        }

        return LoadUnmanagedDllFromPath(libraryPath);
    }
}