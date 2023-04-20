using KarolK72.LegoAssignment.Library.Commands;

namespace KarolK72.LegoAssignment.Library
{
    /// <summary>
    /// This interface describes a service that connects and allows to communicate with the EV3 robot
    /// </summary>
    public interface IEV3CommunicationService : IDisposable
    {
        /// <summary>
        /// Initiates a socket connection with the robot.
        /// </summary>
        /// <param name="url">IP address or hostname to connect to</param>
        /// <param name="port">Host port</param>
        Task Connect(string url, int port);
        /// <summary>
        /// Dispatches the payload to the robot over the socket connection.
        /// </summary>
        /// <param name="payload">Payload to transmit</param>
        Task Dispatch(Payload payload);
        /// <summary>
        /// Disconnects and closes the socket connection.
        /// </summary>
        Task Disconnect();
        /// <summary>
        /// Allows the developer to register a callback that handles a command that the service has received and parsed.
        /// </summary>
        /// <param name="commandType">The type of a command that implements the <see cref="IUpstreamCommand"/> interface.</param>
        /// <param name="handler">An async callback that receives the command (which has parsed the payload) and does some processing</param>
        /// <returns>A disposable object that when disposed will un-register the callback</returns>
        IDisposable? RegisterHandler(Type commandType, Func<IUpstreamCommand, Task> handler);
    }
}