namespace KarolK72.LegoAssignment.Library.Commands
{
    /// <summary>
    /// Interface that descrives an upstream (robot -> client) commands
    /// </summary>
    public interface IUpstreamCommand
    {
        /// <summary>
        /// If null, the command hasn't parsed a payload yet. If not null, the value represents whether the command was able to parse the payload.
        /// </summary>
        bool? IsValid { get; }
        /// <summary>
        /// Parses the payload and configures the object properties accordingly.
        /// </summary>
        /// <param name="payload">Payload to parse</param>
        void ParsePayload(Payload payload);
    }
}
