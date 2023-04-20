using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.Library.Commands.Downstream
{

    [Command(1)]
    public class GetCurrentConfigurationCommand : IDownstreamCommand
    {
        public Payload ConvertToPayload()
        {
            return new Payload() { CommandID = this.GetType().GetCustomAttribute<CommandAttribute>()!.CommandID };
        }
    }
}
