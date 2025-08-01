using System;
using System.Collections.Generic;
using System.Linq;
using Zorro.Core.CLI;

namespace Tweaks.Features.BetterConsole
{
    [TypeParser(typeof(bool))]
    public class BoolParser : CLITypeParser
    {
        private readonly HashSet<string> tVals = new(StringComparer.OrdinalIgnoreCase) { "true", "on", "1", "yes", "enable", "accept" };
        private readonly HashSet<string> fVals = new(StringComparer.OrdinalIgnoreCase) { "false", "off", "0", "no", "disable", "deny" };

        public override object Parse(string str)
        {
            if (tVals.Contains(str))
                return true;
            if (fVals.Contains(str))
                return false;

            throw new ArgumentException($"Could not parse '{str}' as a Boolean value. Expected true/false, on/off, etc.");
        }

        public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
        {
            List<ParameterAutocomplete> suggestions = [];
            if ("true".StartsWith(parameterText, StringComparison.OrdinalIgnoreCase))
                suggestions.Add(new("true"));
            if ("false".StartsWith(parameterText, StringComparison.OrdinalIgnoreCase))
                suggestions.Add(new("false"));
            return suggestions;
        }
    }

    [TypeParser(typeof(Player))]
    public class PlayerParser : CLITypeParser
    {
        public override List<ParameterAutocomplete> FindAutocomplete(string parameterText)
        {
            return [.. PlayerHandler.instance.players
                .Where(p => p.refs?.view?.Owner?.NickName != null && p.refs.view.Owner.NickName.StartsWith(parameterText, StringComparison.OrdinalIgnoreCase))
                .Select(p => new ParameterAutocomplete(p.refs.view.Owner.NickName))];
        }

        public override object Parse(string str)
        {
            return PlayerHandler.instance.players.FirstOrDefault(p => p.refs?.view?.Owner?.NickName.Equals(str, StringComparison.OrdinalIgnoreCase) ?? false) ?? throw new ArgumentException($"No player with username '{str}' exists");
        }
    }
}
