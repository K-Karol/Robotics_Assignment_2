using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KarolK72.LegoAssignment.Library
{
    public class Payload
    {
        public UInt16 CommandID { get; set; } = 0;
        public Dictionary<string,string> Paramaters { get; set; } = new Dictionary<string, string>();

        public override string ToString()
        {
            return $"#{CommandID}{(Paramaters.Count > 0 ? $"{String.Join("",Paramaters.Select(kvp => $"|{kvp.Key}:{kvp.Value}"))}" : "")};";
        }

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

            foreach ( Match kvpMatch in kvpMatches)
            {
                payload.Paramaters.Add(kvpMatch.Groups["K"].Value, kvpMatch.Groups["V"].Value);
            }

            return payload;
        }

        public bool Equals(Payload? obj)
        {
            if(obj is null) return false;

            if(this.CommandID  != obj.CommandID) return false;

            if(this.Paramaters.Count != obj.Paramaters.Count) return false;

            foreach( var kvp in this.Paramaters)
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
