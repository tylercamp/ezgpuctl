using Newtonsoft.Json;
using NvAPIWrapper.GPU;
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

        public static readonly string DefaultProfileName = "Default";
    }
}
