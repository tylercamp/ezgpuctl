using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Model;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.ViewModels
{
    public partial class GpuOverclockProfileViewModel : ObservableObject
    {
        // only for XAML preview
        public GpuOverclockProfileViewModel()
        {
            name = "Profile Name";
            IsReadOnly = false;

            Overclocks = new ReadOnlyCollection<GpuOverclockViewModel>(
                new List<GpuOverclockViewModel>() { new GpuOverclockViewModel() }
            );
        }

        private GpuOverclockProfileViewModel(GpuOverclockProfile profile, IEnumerable<GpuOverclockViewModel> overclocks)
        {
            name = profile.Name;
            Overclocks = new ReadOnlyCollection<GpuOverclockViewModel>(overclocks.ToList());
            IsReadOnly = true;
        }

        public GpuOverclockProfileViewModel(List<PhysicalGPU> gpus, GpuOverclockProfile profile)
        {
            name = profile.Name;

            IsReadOnly = false;
            Overclocks = new ReadOnlyCollection<GpuOverclockViewModel>(
                // use gpus.Select instead of direct profile.OverclockSettings so
                // we can keep the GPU ordering consistent in case a GPU was added or
                // removed
                gpus.Select(gpu =>
                {
                    var wrapper = new GpuWrapper(gpu);
                    var existingOc = profile.OverclockSettings.Where(s => s.GpuId == gpu.GPUId).FirstOrDefault();
                    if (existingOc != null)
                    {
                        return new GpuOverclockViewModel(wrapper, existingOc);
                    }
                    else
                    {
                        var newOc = new GpuOverclock { GpuId = gpu.GPUId };
                        return new GpuOverclockViewModel(wrapper, newOc);
                    }
                }).ToList()
            );
        }

        public static GpuOverclockProfileViewModel GetDefault(List<PhysicalGPU> gpus)
        {
            var overclockVms = gpus.Select(gpu => GpuOverclockViewModel.GetDefault(gpu)).ToList();

            var profile = new Model.GpuOverclockProfile("Default Profile")
            {
                OverclockSettings = overclockVms.Select(vm => vm.AsModelObject).ToList()
            };

            return new GpuOverclockProfileViewModel(profile, overclockVms);
        }

        public bool IsReadOnly { get; private set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Label))]
        private string name;

        public string Label
        {
            get
            {
                var ocSummary = Overclocks
                    .Select(oc =>
                    {
                        var parts = new List<string>();
                        if (oc.IsStock) parts.Add("stock settings");
                        else
                        {
                            if (oc.UsesPowerTarget) parts.Add($"power: {oc.PowerTarget}%");
                            if (oc.UsesCoreClockOffset) parts.Add($"core: {oc.CoreClockOffset}MHz");
                            if (oc.UsesMemoryClockOffset) parts.Add($"memory: {oc.MemoryClockOffset}MHz");
                        }

                        if (parts.Count > 0)
                        {
                            return $"[#{oc.GpuId} - {string.Join(", ", parts)}]";
                        }
                        else
                        {
                            return "";
                        }
                    })
                    .Where(s => s.Length > 0)
                    .ToList();

                if (ocSummary.Count > 0) return $"{Name} {string.Join(" ", ocSummary)}";
                else return $"{Name} (no OCs applied)";
            }
        }

        public ReadOnlyCollection<GpuOverclockViewModel> Overclocks { get; private set; }

        public GpuOverclockProfile AsModelObject => new GpuOverclockProfile(name)
        {
            OverclockSettings = Overclocks.Select(ovm => ovm.AsModelObject).ToList()
        };
    }
}
