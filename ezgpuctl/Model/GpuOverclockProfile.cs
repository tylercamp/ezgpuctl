using GPUControl.Lib.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Model
{
    public class GpuOverclockProfile
    {
        public GpuOverclockProfile(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<GpuOverclock> OverclockSettings { get; set; } = new List<GpuOverclock>();

        public GpuOverclockProfile Clone() => new GpuOverclockProfile(Name) { OverclockSettings = OverclockSettings.Select(oc => oc.Clone()).ToList() };
    }
}
