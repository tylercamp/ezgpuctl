using GPUControl.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock.Result
{
    public class ProfileResult : IBehaviorResult
    {
        public ProfileResult(List<GpuOverclock> overclocks, string profileName)
        {
            AppliedOverclocks = overclocks;
            ProfileName = profileName;
        }

        public string ProfileName { get; }
        public List<GpuOverclock> AppliedOverclocks { get; }
    }
}
