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
            IsFirstRun = false;
        }

        public Settings(string? path)
        {
            Path = path;
            // newtonsoft seems to call this ctor with `null`
            IsFirstRun = path != null;
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

        public bool HideOnStartup { get; set; } = false;

        public bool PauseOcService { get; set; } = false;

        public bool AskBeforeClose { get; set; } = true;

        [JsonIgnore]
        public bool IsFirstRun { get; }

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

            // remove entries with conflicting names, remove references to profiles and policies which
            // don't exist with the given name

            result.Profiles = result.Profiles.Where(p => p == result.Profiles.Last(l => l.Name == p.Name)).ToList();
            result.Policies = result.Policies.Where(p => p == result.Policies.Last(l => l.Name == p.Name)).ToList();

            foreach (var p in result.Policies)
            {
                // remove duplicates and any profile names which do not exist
                p.OrderedProfileNames = p.OrderedProfileNames.Where(n => result.Profiles.Any(p => p.Name == n)).Distinct().ToList();
            }

            if (result.OcMode_SpecificPolicyName != null && !result.Policies.Any(p => p.Name == result.OcMode_SpecificPolicyName))
                result.OcMode_SpecificPolicyName = null;

            if (result.OcMode_SpecificProfileName != null && !result.Profiles.Any(p => p.Name == result.OcMode_SpecificProfileName))
                result.OcMode_SpecificProfileName = null;

            return result;
        }
    }
}
