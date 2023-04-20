using System.Text.RegularExpressions;

namespace KarolK72.LegoAssignment.Library
{
    /// <summary>
    /// Class that represents the data transmitted over a socket connection
    /// </summary>
    public class Payload
    {
        /// <summary>
        /// The ID of the command
        /// </summary>
        public UInt16 CommandID { get; set; } = 0;
        /// <summary>
        /// A Dictionary of keys (strings) and values (string) which are the arguments/data of a payload.
        /// </summary>
        public Dictionary<string, string> Paramaters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Encodes the payload as a string so it may be transmitted.
        /// </summary>
        /// <returns>Payload encoded as a string</returns>
        public override string ToString()
        {
            return $"#{CommandID}{(Paramaters.Count > 0 ? $"{String.Join("", Paramaters.Select(kvp => $"|{kvp.Key}:{kvp.Value}"))}" : "")};";
        }

        /// <summary>
        /// Static function that parses a string receieved over a socket connection into a Payload.
        /// If the received string is not valid, null is returned.
        /// </summary>
        /// <param name="toParse">Payload encoded as a string</param>
        /// <returns>If valid, a Payload object that contains all of the details, otherwise null if not valid</returns>
        public static Payload? Parse(string toParse)
        {
            Regex payloadRegex = new Regex(@"#(?<CommandID>\d+)(?:\|[^\s\:\|]+\:[^\s\:\|]+)*;", RegexOptions.Compiled);
            Regex kvpRegex = new Regex(@"(?<K>[^\s\:\|\;]+)\:(?<V>[^\s\:\|\;]+)", RegexOptions.Compiled);
            var match = payloadRegex.Match(toParse);
            if (!match.Success || !match.Groups.ContainsKey("CommandID"))
            {
                return null;
            }

            var payload = new Payload() { CommandID = UInt16.Parse(match.Groups["CommandID"].Value) };

            var kvpMatches = kvpRegex.Matches(toParse);

            foreach (Match kvpMatch in kvpMatches)
            {
                payload.Paramaters.Add(kvpMatch.Groups["K"].Value, kvpMatch.Groups["V"].Value);
            }

            return payload;
        }

        /// <summary>
        /// Allows for comparing two payloads, used primarily in unit testing.
        /// </summary>
        /// <param name="obj">Payload to compare against</param>
        /// <returns>True if the payloads contain matching details</returns>
        public bool Equals(Payload? obj)
        {
            if (obj is null) return false;

            if (this.CommandID != obj.CommandID) return false;

            if (this.Paramaters.Count != obj.Paramaters.Count) return false;

            foreach (var kvp in this.Paramaters)
            {
                if (obj.Paramaters.TryGetValue(kvp.Key, out string? value))
                {
                    if (!kvp.Value.Equals(value))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
