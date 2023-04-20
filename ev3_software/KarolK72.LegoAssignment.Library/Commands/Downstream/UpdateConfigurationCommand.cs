using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.Library.Commands.Downstream
{
    [Command(2)]
    public class UpdateConfigurationCommand : IDownstreamCommand
    {
        public bool IsBlackList { private set; get; }
        public List<string> ColourList { private set; get; } = new List<string>();

        public UpdateConfigurationCommand(bool isBlackList, List<string> colourList) {
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
