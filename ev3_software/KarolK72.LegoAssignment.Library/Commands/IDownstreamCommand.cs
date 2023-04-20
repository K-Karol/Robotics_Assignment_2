namespace KarolK72.LegoAssignment.Library.Commands
{
    /// <summary>
    /// A class that represents a downstream (client -> robot) command.
    /// </summary>
    public interface IDownstreamCommand
    {
        /// <summary>
        /// Converts the command details (properties) into a payload
        /// </summary>
        /// <returns>Payload with the details of the command</returns>
        Payload ConvertToPayload();
    }
}
