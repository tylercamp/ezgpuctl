using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GPUControl.Model
{
    public class ProgramPolicyRule
    {
        public bool Negated { get; set; }
        public string ProgramName { get; set; }

        public bool IsRegex { get; set; } = false;

        public bool CaseInsensitive { get; set; } = false;

        public bool IsApplicable(List<string> programNames)
        {
            bool matched;
            if (IsRegex)
            {
                var regexFlags = CaseInsensitive ? RegexOptions.IgnoreCase : RegexOptions.None;
                var regex = new Regex(ProgramName, regexFlags);
                matched = programNames.Any(n => regex.IsMatch(n));
            }
            else
            {
                matched = programNames.Any(n => CaseInsensitive ? n.ToLower() == ProgramName.ToLower() : n == ProgramName);
            }

            if (Negated) matched = !matched;
            return matched;
        }
    }
}
