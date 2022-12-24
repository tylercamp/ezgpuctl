using GPUControl.Lib.GPU;
using GPUControl.Overclock.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock
{
    public interface IOverclockBehavior
    {
        Task<IBehaviorResult> Apply(List<IGpuWrapper> gpus);
    }
}
