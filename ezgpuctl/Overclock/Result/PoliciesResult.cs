using GPUControl.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock.Result
{
    public class PoliciesResult : IBehaviorResult
    {
        public PoliciesResult(List<GpuOverclock> overclocks, List<string> policyNames)
        {
            this.AppliedPolicyNames = policyNames;
            this.AppliedOverclocks = overclocks;
        }

        public List<string> AppliedPolicyNames { get; }
        public List<GpuOverclock> AppliedOverclocks { get; }
    }
}
