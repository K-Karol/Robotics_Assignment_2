namespace KarolK72.LegoAssignment.Library.Commands.Upstream
{
    /// <summary>
    /// This command contains the details of the latest scanned item by the robot incl. the colour and whether it was rejected or not.
    /// </summary>
    [Command(1)]
    public class DetectedCommand : IUpstreamCommand
    {
        private bool _isRejected = false;
        /// <summary>
        /// True/False whether the item was rejected
        /// </summary>
        public bool IsRejected { get { return _isRejected; } private set { _isRejected = value; } }

        private string _colour = string.Empty;
        /// <summary>
        /// The colour of the detected item
        /// </summary>
        public string Colour { get { return _colour; } private set { _colour = value; } }
        private bool? _isValid;
        public bool? IsValid => _isValid;

        public void ParsePayload(Payload payload)
        {
            if (payload.Paramaters.ContainsKey(nameof(Colour)))
            {
                _colour = payload.Paramaters[nameof(Colour)];
            }
            else
            {
                _isValid = false;
            }

            if (payload.Paramaters.ContainsKey(nameof(IsRejected)))
            {
                switch (payload.Paramaters[nameof(IsRejected)].ToLower())
                {
                    case "true":
                        _isRejected = true;
                        break;
                    case "false":
                        _isRejected = false;
                        break;
                    default:
                        _isValid = false;
                        break;
                }
            }

            if (_isValid is null) _isValid = true;

        }
    }
}
