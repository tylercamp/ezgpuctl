using GPUControl.Lib.GPU;
using GPUControl.Lib.Model;
using GPUControl.Model;
using GPUControl.Overclock.Result;
using GPUControl.Util;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Overclock
{
    public class MultiPolicyBehavior : IOverclockBehavior
    {
        private ILogger logger;
        private Func<Task<Settings>> settingsGetter;

        public MultiPolicyBehavior(Func<Task<Settings>> settingsGetter)
        {
            this.logger = Log.ForContext<MultiPolicyBehavior>();
            this.settingsGetter = settingsGetter;
        }

        public async Task<IBehaviorResult> Apply(List<IGpuWrapper> gpus)
        {
            var programNames = ProcessMonitor.CurrentProgramNames();

            var currentSettings = await settingsGetter();

            var matchingPolicies = currentSettings.Policies.Where(p => p.Rules.All(r => r.IsApplicable(programNames))).ToList();

            logger.Debug("Matching policies: {0}", matchingPolicies.Select(p => p.Name));

            var matchingProfiles = matchingPolicies.SelectMany(currentSettings.ProfilesForPolicy).ToList();

            var ocs = matchingProfiles.SelectMany(profile => profile.OverclockSettings).ToList();

            logger.Debug("Found {0} profiles and {0} OCs", matchingProfiles.Count, ocs.Count(oc => oc.HasOcSettings));

            var finalOcs = gpus.Select(gpu => GpuOverclock.Merge(gpu.GpuId, ocs)).ToList();

            logger.Debug("Created final OCs: {0}", finalOcs);
            foreach (var oc in finalOcs)
            {
                var wrapper = gpus.Single(gpu => gpu.GpuId == oc.GpuId);
                wrapper.ApplyOC(oc);
            }

            return new PoliciesResult(matchingPolicies.Select(p => p.Name).ToList());
        }
    }
}
