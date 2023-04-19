using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.Library.Commands
{
    public class CommandAttribute : Attribute
    {
        private int _commandID = 0;
        public int CommandID { get { return _commandID; } }
        public CommandAttribute(int commandID)
        {
            _commandID = commandID;
        }
    }
}
