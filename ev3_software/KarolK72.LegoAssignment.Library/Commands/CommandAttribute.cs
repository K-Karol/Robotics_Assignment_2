using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.Library.Commands
{
    public class CommandAttribute : Attribute
    {
        private UInt16 _commandID = 0;
        public UInt16 CommandID { get { return _commandID; } }
        public CommandAttribute(UInt16 commandID)
        {
            _commandID = commandID;
        }
    }
}
