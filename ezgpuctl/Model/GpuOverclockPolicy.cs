using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Model
{
    public class GpuOverclockPolicy
    {
        public List<ProgramPolicyRule> Rules { get; set; } = new List<ProgramPolicyRule>();
        public List<string> OrderedProfileNames { get; set; } = new List<string>();
    }
}
