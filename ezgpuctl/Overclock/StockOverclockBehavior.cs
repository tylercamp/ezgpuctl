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
            var policyNames = new List<string>() { GpuOverclockPolicyViewModel.DefaultName };
            var overclocks = gpus
                .Select(gpu =>
                {
                    var oc = GpuOverclock.Stock(gpu.GpuId);
                    gpu.ApplyOC(oc);
                    return oc;
                })
                .ToList();

            return Task.FromResult<IBehaviorResult>(new PoliciesResult(overclocks, policyNames));
        }
    }
}
