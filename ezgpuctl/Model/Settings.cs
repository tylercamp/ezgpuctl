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

        public void Save() => File.WriteAllText(Path, JsonConvert.SerializeObject(this));

        public static Settings LoadFrom(string filename)
        {
            if (!File.Exists(filename)) return new Settings(filename);

            var result = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filename)) ?? new Settings(filename);
            result.Path = filename;

            return result;
        }
    }
}
