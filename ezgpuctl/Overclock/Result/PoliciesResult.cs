using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock.Result
{
    public class PoliciesResult : IBehaviorResult
    {
        public PoliciesResult(List<string> policyNames)
        {
            this.AppliedPolicyNames = policyNames;
        }

        public List<string> AppliedPolicyNames { get; }
    }
}
