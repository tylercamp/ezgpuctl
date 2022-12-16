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
            _pendingLabel = "Profile Label";
            IsReadOnly = false;

            Overclocks = new ReadOnlyCollection<GpuOverclockViewModel>(
                new List<GpuOverclockViewModel>() { new GpuOverclockViewModel() }
            );
        }

        private GpuOverclockProfileViewModel(GpuOverclockProfile profile, IEnumerable<GpuOverclockViewModel> overclocks)
        {
            this.profile = profile;
            _pendingLabel = profile.Label;
            Overclocks = new ReadOnlyCollection<GpuOverclockViewModel>(overclocks.ToList());
            IsReadOnly = true;
        }

        public GpuOverclockProfileViewModel(List<PhysicalGPU> gpus, GpuOverclockProfile profile)
        {
            this.profile = profile;
            _pendingLabel = profile.Label;

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
                        return new GpuOverclockViewModel(wrapper, new GpuOverclock
                        {
                            GpuId = gpu.GPUId
                        });
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

        private string _pendingLabel;
        public string Label
        {
            get => _pendingLabel;
            set
            {
                if (IsReadOnly) throw new InvalidOperationException();

                _pendingLabel = value;
                OnPropertyChanged();
            }
        }

        public void ApplyPendingLabel()
        {
            profile.Label = _pendingLabel;
        }

        public void RevertPendingLabel()
        {
            Label = profile.Label;
        }

        public ReadOnlyCollection<GpuOverclockViewModel> Overclocks { get; private set; }

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
