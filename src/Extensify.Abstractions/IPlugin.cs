namespace Extensify.Abstractions;

/// <summary>
/// Defines the shared contract that every Extensify plugin must implement
/// </summary>
public interface IPlugin
{ 
    /// <summary>
    /// Gets the human-readable plugin name.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets a short description of what the plugin does.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Gets the command used to invoke the plugin from the host.
    /// </summary>
    string Command { get; }

    /// <summary>
    /// Executes the plugin with the provided command arguments.
    /// </summary>
    /// <param name="args">The command arguments passed by the host.</param>
    /// <returns>The execution result.</returns>
    PluginExecutionResult Execute(string[] args);
}

/// <summary>
/// Represents the outcome of a plugin execution.
/// </summary>
public sealed record PluginExecutionResult
{
    /// <summary>
    /// Gets a value indicating execution completed successfully.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the message returned by the plugin (result or error details).
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Creates a successful execution result.
    /// </summary>
    /// <param name="message">The success message.</param>
    /// <returns>A successful <see cref="PluginExecutionResult"/>.</returns>
    public static PluginExecutionResult Success(string message) =>
        new() { IsSuccess = true, Message = message };

    public static PluginExecutionResult Failure(string message) =>
        new() { IsSuccess = false, Message = message };
}