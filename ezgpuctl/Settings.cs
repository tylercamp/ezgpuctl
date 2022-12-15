using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl
{
    public class Settings
    {
        private Settings()
        {
            Path = "";
        }

        public Settings(string path)
        {
            Path = path;
        }

        [JsonIgnore]
        public string Path { get; private set; }

        public List<Model.GpuOverclockProfile> Profiles { get; set; } = new List<Model.GpuOverclockProfile>();
        public List<Model.GpuOverclockPolicy> Policies { get; set; } = new List<Model.GpuOverclockPolicy>();

        public List<Model.GpuOverclockProfile> ProfilesForPolicy(Model.GpuOverclockPolicy policy) =>
            (
                from name in policy.OrderedProfileNames
                from profile in Profiles
                where profile.Label == name
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
