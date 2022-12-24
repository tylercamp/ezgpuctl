using Newtonsoft.Json;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Model
{
    public class Settings
    {
        private Settings()
        {
        }

        public Settings(string? path)
        {
            Path = path;
        }

        public enum OcModeType
        {
            Stock,
            Policies,
            SpecificPolicy,
            SpecificProfile
        }

        public OcModeType OcMode { get; set; } = OcModeType.Policies;

        public string? OcMode_SpecificProfileName { get; set; }
        public string? OcMode_SpecificPolicyName { get; set; }

        [JsonIgnore]
        public string? Path { get; private set; }

        public List<GpuOverclockProfile> Profiles { get; private set; } = new List<GpuOverclockProfile>();

        public List<GpuOverclockPolicy> Policies { get; private set; } = new List<GpuOverclockPolicy>();


        public List<GpuOverclockProfile> ProfilesForPolicy(GpuOverclockPolicy policy) =>
            (
                from name in policy.OrderedProfileNames
                from profile in Profiles
                where profile.Name == name
                select profile
            ).ToList();

        public bool HideOnStartup { get; set; }

        public bool PauseOcService { get; set; }

        public bool AskBeforeClose { get; set; } = true;

        private Settings Sanitized
        {
            get
            {
                // remove entries with conflicting names, remove references to profiles and policies which
                // don't exist with the given name

                var result = new Settings(Path);

                var observedProfileNames = new List<string>();
                foreach (var profile in Profiles)
                {
                    if (observedProfileNames.Contains(profile.Name)) continue;

                    result.Profiles.Add(profile);
                    observedProfileNames.Add(profile.Name);
                }

                var observedPolicyNames = new List<string>();
                foreach (var policy in Policies)
                {
                    if (observedPolicyNames.Contains(policy.Name)) continue;

                    policy.OrderedProfileNames = policy.OrderedProfileNames.Where(observedProfileNames.Contains).Distinct().ToList();
                    result.Policies.Add(policy);
                    observedPolicyNames.Add(policy.Name);
                }

                result.OcMode = OcMode;
                result.PauseOcService = PauseOcService;
                result.HideOnStartup = HideOnStartup;
                result.AskBeforeClose = AskBeforeClose;

                if (OcMode_SpecificPolicyName != null && observedPolicyNames.Contains(OcMode_SpecificPolicyName))
                    result.OcMode_SpecificPolicyName = OcMode_SpecificPolicyName;

                if (OcMode_SpecificProfileName != null && observedProfileNames.Contains(OcMode_SpecificProfileName))
                    result.OcMode_SpecificProfileName = OcMode_SpecificProfileName;

                return result;
            }
        }

        public void Save()
        {
            if (Path == null) throw new InvalidOperationException();

            File.WriteAllText(Path, JsonConvert.SerializeObject(this));
        }

        public static Settings LoadFrom(string filename)
        {
            if (!File.Exists(filename)) return new Settings(filename);

            var result = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filename)) ?? new Settings(filename);
            result.Path = filename;

            return result.Sanitized;
        }
    }
}
