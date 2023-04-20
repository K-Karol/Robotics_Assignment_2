using System.Reflection;

namespace KarolK72.LegoAssignment.Library.Commands.Downstream
{
    /// <summary>
    /// Downstream command that requests the robot to transmit a payload that represents the <see cref="Commands.Upstream.CurrentConfigurationCommand"/>
    /// </summary>
    [Command(1)]
    public class GetCurrentConfigurationCommand : IDownstreamCommand
    {
        public Payload ConvertToPayload()
        {
            return new Payload() { CommandID = this.GetType().GetCustomAttribute<CommandAttribute>()!.CommandID };
        }
    }
}
