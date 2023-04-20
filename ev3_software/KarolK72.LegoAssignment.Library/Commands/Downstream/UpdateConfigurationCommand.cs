using System.Reflection;

namespace KarolK72.LegoAssignment.Library.Commands.Downstream
{
    /// <summary>
    /// This command contains the newest configuration details that are to be sent to the robot. Paramaters same as <see cref="Commands.Upstream.CurrentConfigurationCommand"/>
    /// </summary>
    [Command(2)]
    public class UpdateConfigurationCommand : IDownstreamCommand
    {
        public bool IsBlackList { private set; get; }
        public List<string> ColourList { private set; get; } = new List<string>();

        public UpdateConfigurationCommand(bool isBlackList, List<string> colourList)
        {
            IsBlackList = isBlackList;
            ColourList = colourList;
        }

        public Payload ConvertToPayload()
        {
            return new Payload()
            {
                CommandID = this.GetType().GetCustomAttribute<CommandAttribute>()!.CommandID,
                Paramaters = new Dictionary<string, string>() {
                    { nameof(IsBlackList), IsBlackList ? "true" : "false" },
                    { nameof(ColourList), string.Join(',', ColourList)}
                }
            };
        }
    }
}
