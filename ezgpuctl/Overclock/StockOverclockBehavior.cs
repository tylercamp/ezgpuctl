using GPUControl.Lib.GPU;
using GPUControl.Lib.Model;
using GPUControl.Overclock.Result;
using GPUControl.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock
{
    public class StockOverclockBehavior : IOverclockBehavior
    {
        public Task<IBehaviorResult> Apply(List<IGpuWrapper> gpus)
        {
            foreach (var gpu in gpus)
            {
                gpu.ApplyOC(GpuOverclock.Stock(gpu.GpuId));
            }

            return Task.FromResult<IBehaviorResult>(new PoliciesResult(new List<string>() { GpuOverclockPolicyViewModel.DefaultName }));
        }
    }
}
