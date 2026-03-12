using Extensify.Abstractions;

namespace Extensify.Loader;

/// <summary>
/// Represents the result of scanning and loading plugin assemblies.
/// </summary>
public sealed class PluginLoadResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLoadResult"/> class.
    /// </summary>
    /// <param name="plugins">The successfully loaded plugin instances.</param>
    /// <param name="errors">The non-fatal errors collected during loading.</param>
    public PluginLoadResult(IReadOnlyList<IPlugin> plugins, IReadOnlyList<PluginLoadError> errors)
    {
        Plugins = plugins;
        Errors = errors;
    }
    
    /// <summary>
    /// Gets the successfully loaded plugin instances.
    /// </summary>
    public IReadOnlyList<IPlugin> Plugins { get; }
    
    /// <summary>
    /// Gets the non-fatal loading errors.
    /// </summary>
    public IReadOnlyList<PluginLoadError> Errors { get; }
}

/// <summary>
/// Describes one plugin loading error.
/// </summary>
public sealed class PluginLoadError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLoadError"/> class.
    /// </summary>
    /// <param name="source">The assembly path or type name that caused the error.</param>
    /// <param name="message">A readable error message.</param>
    public PluginLoadError(string source, string message)
    {
        Source = source;
        Message = message;
    }
    
    /// <summary>
    /// Gets the assembly path or type name that caused the error.
    /// </summary>
    public string Source { get; }
    
    /// <summary>
    /// Gets the readable error message.
    /// </summary>
    public string Message { get; }
}