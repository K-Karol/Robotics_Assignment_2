using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.Library.Commands.Upstream
{
    [Command(2)]
    public class CurrentConfigurationCommand : IUpstreamCommand
    {
        private bool _isBlacklist = false;
        public bool IsBlackList { get { return _isBlacklist; } private set { _isBlacklist = value; } }

        private List<string> _colourList = new List<string>();
        public List<string> ColourList { get { return _colourList; } private set { _colourList = value; } }

        private bool? _isValid;
        public bool? IsValid => _isValid;

        public void ParsePayload(Payload payload)
        {
            if (payload.Paramaters.ContainsKey(nameof(IsBlackList)))
            {
                switch (payload.Paramaters[nameof(IsBlackList)].ToLower())
                {
                    case "true":
                        _isBlacklist = true;
                        break;
                    case "false":
                        _isBlacklist = false;
                        break;
                    default:
                        _isValid = false;
                        break;
                }
            } else
            {
                _isValid = false;
            }

            if (payload.Paramaters.ContainsKey(nameof(ColourList)))
            {
                _colourList = payload.Paramaters[nameof(ColourList)].Split(",").ToList();
            } else
            {
                _isValid = false;
            }

            if (_isValid is null) _isValid = true;
        }
    }
}
