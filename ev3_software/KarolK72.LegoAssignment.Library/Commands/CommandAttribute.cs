namespace KarolK72.LegoAssignment.Library.Commands
{
    /// <summary>
    /// Used to decorate commands with their command ID.
    /// </summary>
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
