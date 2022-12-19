using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Model
{
    public class ProgramPolicyRule
    {
        public bool Negated { get; set; }
        public string ProgramName { get; set; }

        public bool IsApplicable(List<string> programNames)
        {
            var contains = programNames.Contains(ProgramName);
            if (Negated) contains = !contains;
            return contains;
        }

        public ProgramPolicyRule Clone() => new ProgramPolicyRule() { ProgramName = ProgramName, Negated = Negated };
    }
}
