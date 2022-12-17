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
    public class GpuOverclockProfileViewModel : ViewModel
    {
        GpuOverclockProfile profile;

        // only for XAML preview
        public GpuOverclockProfileViewModel()
        {
            _pendingName = "Profile Name";
            IsReadOnly = false;

            Overclocks = new ReadOnlyCollection<GpuOverclockViewModel>(
                new List<GpuOverclockViewModel>() { new GpuOverclockViewModel() }
            );
        }

        private GpuOverclockProfileViewModel(GpuOverclockProfile profile, IEnumerable<GpuOverclockViewModel> overclocks)
        {
            this.profile = profile;
            _pendingName = profile.Name;
            Overclocks = new ReadOnlyCollection<GpuOverclockViewModel>(overclocks.ToList());
            IsReadOnly = true;
        }

        public GpuOverclockProfileViewModel(List<PhysicalGPU> gpus, GpuOverclockProfile profile)
        {
            this.profile = profile;
            _pendingName = profile.Name;

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
                        this.profile.OverclockSettings.Add(newOc);
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
                OverclockSettings = overclockVms.Select(vm => vm.OverclockPreview).ToList()
            };

            return new GpuOverclockProfileViewModel(profile, overclockVms);
        }

        public bool IsReadOnly { get; private set; }

        private string _pendingName;
        public string Name
        {
            get => _pendingName;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                _pendingName = value;
                OnPropertyChanged();
            }
        }

        public void ApplyPendingName()
        {
            var oldName = profile.Name;
            profile.Name = _pendingName;

            NameSaved?.Invoke(oldName, profile.Name);
        }

        public void RevertPendingName()
        {
            Name = profile.Name;
        }

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

        // (OldName, NewName)
        public event Action<string, string> NameSaved;

        public bool HasChanges => Overclocks.Any(oc => oc.HasChanges);
        public void ApplyChanges()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            foreach (var ocVm in Overclocks)
                ocVm.ApplyChanges();
        }

        public void RevertChanges()
        {
            if (IsReadOnly) throw new InvalidOperationException();

            foreach (var ocVm in Overclocks)
                ocVm.RevertChanges();
        }
    }
}
