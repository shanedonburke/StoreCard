namespace StoreCard.Commands;

/// <summary>
/// Represents an executable command.
/// </summary>
/// <typeparam name="TResult">
/// The type of the value returned by the command's execution
/// </typeparam>
public interface IStoreCardCommand<out TResult>
{
    /// <summary>
    /// Exeutes the command.
    /// </summary>
    /// <returns>The execution result, the significance of which is defined by each command</returns>
    public TResult Execute();
}
