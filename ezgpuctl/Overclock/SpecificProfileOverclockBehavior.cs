using GPUControl.Lib.GPU;
using GPUControl.Lib.Model;
using GPUControl.Model;
using GPUControl.Overclock.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock
{
    public class SpecificProfileOverclockBehavior : IOverclockBehavior
    {
        private Func<Task<Settings>> settingsGetter;
        private string? profileName;

        public SpecificProfileOverclockBehavior(Func<Task<Settings>> settingsGetter, string? profileName)
        {
            this.settingsGetter = settingsGetter;
            this.profileName = profileName;
        }

        public string? ProfileName => profileName;

        public async Task<IBehaviorResult> Apply(List<IGpuWrapper> gpus)
        {
            var settings = await settingsGetter();
            var profile = settings.Profiles.FirstOrDefault(profile => profile.Name == profileName);
            if (profile != null)
            {
                var ocs = profile.OverclockSettings.ToList();
                ocs.AddRange(gpus.Select(gpu => GpuOverclock.Stock(gpu.GpuId)));

                var finalOcs = gpus.Select(gpu => GpuOverclock.Merge(gpu.GpuId, ocs)).ToList();
                foreach (var oc in finalOcs)
                {
                    var wrapper = gpus.Single(gpu => gpu.GpuId == oc.GpuId);
                    wrapper.ApplyOC(oc);
                }

                return new ProfileResult(finalOcs, profile.Name);
            }
            else
            {
                return new PoliciesResult(new List<GpuOverclock>(), new List<string>());
            }
        }
    }
}
