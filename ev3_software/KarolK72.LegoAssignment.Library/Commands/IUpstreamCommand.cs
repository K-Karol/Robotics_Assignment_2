using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.Library.Commands
{
    public interface IUpstreamCommand
    {
        bool? IsValid { get; }
        void ParsePayload(Payload payload);
    }
}
