using GPUControl.Lib.GPU;
using GPUControl.Lib.Model;
using GPUControl.Model;
using GPUControl.Overclock.Result;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock
{
    class SpecificPolicyOverclockBehavior : IOverclockBehavior
    {
        private ILogger logger;
        private Func<Task<Settings>> settingsGetter;
        private string? policyName;

        public SpecificPolicyOverclockBehavior(Func<Task<Settings>> settingsGetter, string? policyName)
        {
            logger = Log.ForContext<SpecificPolicyOverclockBehavior>();
            this.settingsGetter = settingsGetter;
            this.policyName = policyName;
        }

        public string? PolicyName => policyName;

        public async Task<IBehaviorResult> Apply(List<IGpuWrapper> gpus)
        {
            var settings = await settingsGetter();
            var policy = settings.Policies.FirstOrDefault(p => p.Name == policyName);
            if (policy != null)
            {
                var profiles = settings.ProfilesForPolicy(policy);
                var ocs = profiles.SelectMany(p => p.OverclockSettings).ToList();
                ocs.AddRange(gpus.Select(gpu => GpuOverclock.Stock(gpu.GpuId)));

                var finalOcs = gpus.Select(gpu => GpuOverclock.Merge(gpu.GpuId, ocs)).ToList();
                foreach (var oc in finalOcs)
                {
                    var wrapper = gpus.Single(gpu => gpu.GpuId == oc.GpuId);
                    wrapper.ApplyOC(oc);
                }

                return new PoliciesResult(finalOcs, new List<string> { policy.Name });
            }
            else
            {
                logger.Warning("Unable to find policy with the name {0}", policyName);
                return new PoliciesResult(new List<GpuOverclock>(), new List<string>());
            }
        }
    }
}
