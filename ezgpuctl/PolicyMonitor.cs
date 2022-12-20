using GPUControl.gpu;
using GPUControl.Model;
using GPUControl.ViewModels;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPUControl
{
    public static class PolicyMonitor
    {
        public static TimeSpan UpdateInterval = TimeSpan.FromSeconds(5);

        private class Context
        {
            public CancellationTokenSource TokenSource { get; set; }
            public Thread CurrentThread { get; set; }
        }

        private static Context? currentContext = null;

        public static event Action<List<string>> PoliciesApplied;

        public static void Start(Settings settingsSource, GpuOverclockPolicy defaultPolicy, GpuOverclockProfile defaultProfile, List<IGpuWrapper> gpus)
        {
            if (currentContext != null) return;

            currentContext = new Context { TokenSource = new CancellationTokenSource() };

            currentContext.CurrentThread = new Thread(() =>
            {
                var logger = Serilog.Log.ForContext(typeof(PolicyMonitor));
                var token = currentContext.TokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    var finalOc = gpus.Select(gpu => new GpuOverclock() { GpuId = gpu.GpuId }).ToList();

                    var currentSettings = settingsSource.Clone();
                    currentSettings.Policies.Add(defaultPolicy);
                    currentSettings.Profiles.Add(defaultProfile);

                    var currentPrograms = ProcessMonitor.ProgramNames!;
                    var matchingPolicies = currentSettings.Policies.Where(p => p.Rules.All(r => r.IsApplicable(currentPrograms))).ToList();

                    logger.Debug("Matching policies: {0}", matchingPolicies.Select(p => p.Name));

                    var matchingProfiles = matchingPolicies.SelectMany(currentSettings.ProfilesForPolicy).ToList();
                        
                    var ocs = matchingProfiles.SelectMany(profile => profile.OverclockSettings).ToList();

                    logger.Debug("Found {0} profiles and {0} OCs", matchingProfiles.Count, ocs.Count(oc => oc.HasOcSettings));

                    foreach (var oc in ocs.Reverse<GpuOverclock>())
                    {
                        foreach (var target in finalOc.Where(foc => foc.GpuId == oc.GpuId))
                        {
                            target.PowerTarget = oc.PowerTarget ?? target.PowerTarget;
                            target.CoreClockOffset = oc.CoreClockOffset ?? target.CoreClockOffset;
                            target.MemoryClockOffset = oc.MemoryClockOffset ?? target.MemoryClockOffset;
                        }
                    }

                    logger.Debug("Created final OCs: {0}", finalOc);
                    foreach (var oc in finalOc)
                    {
                        var wrapper = gpus.Single(gpu => gpu.GpuId == oc.GpuId);
                        wrapper.ApplyOC(oc);
                    }

                    PoliciesApplied?.Invoke(matchingPolicies.Select(p => p.Name).ToList());

                    try { Task.Delay(UpdateInterval, token).Wait(); }
                    catch { break; }
                }
            });

            currentContext.CurrentThread.Start();
        }

        public static void Stop()
        {
            if (currentContext == null) return;

            currentContext.TokenSource.Cancel();
            currentContext.CurrentThread.Join();
        }
    }
}
