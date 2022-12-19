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

        public Settings(string path)
        {
            Path = path;
        }

        [JsonIgnore]
        public string Path { get; private set; }

        public List<GpuOverclockProfile> Profiles { get; private set; } = new List<GpuOverclockProfile>();

        public List<GpuOverclockPolicy> Policies { get; private set; } = new List<GpuOverclockPolicy>();


        public List<GpuOverclockProfile> ProfilesForPolicy(GpuOverclockPolicy policy) =>
            (
                from name in policy.OrderedProfileNames
                from profile in Profiles
                where profile.Name == name
                select profile
            ).ToList();

        public Settings Clone() => new Settings() { Profiles = Profiles.Select(p => p.Clone()).ToList(), Policies = Policies.Select(p => p.Clone()).ToList() };

        private Settings Sanitized
        {
            get
            {
                // remove entries with conflicting names, remove profiles from policies where the no profiles exist
                // with the given name

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

                return result;
            }
        }

        public void Save() => File.WriteAllText(Path, JsonConvert.SerializeObject(this));

        public static Settings LoadFrom(string filename)
        {
            if (!File.Exists(filename)) return new Settings(filename);

            var result = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filename)) ?? new Settings(filename);
            result.Path = filename;

            return result.Sanitized;
        }
    }
}
