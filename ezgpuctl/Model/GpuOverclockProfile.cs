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
        public GpuOverclockProfile(string label, bool isReadOnly = false)
        {
            Label = label;
            IsReadOnly = isReadOnly;
        }

        [JsonIgnore]
        public bool IsReadOnly { get; }

        public string Label { get; set; }
        public List<GpuOverclock> OverclockSettings { get; set; } = new List<GpuOverclock>();
    }
}
