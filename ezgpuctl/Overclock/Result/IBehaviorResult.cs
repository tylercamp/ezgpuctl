using GPUControl.Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock.Result
{
    public interface IBehaviorResult
    {
        List<GpuOverclock> AppliedOverclocks { get; }
    }
}
