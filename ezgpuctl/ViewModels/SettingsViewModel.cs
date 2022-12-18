using GPUControl.Model;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.ViewModels
{
    // note: when presenting a list, the list should not correspond 

    public class SettingsViewModel
    {
        private Settings settings;

        public SettingsViewModel(List<PhysicalGPU> gpus, Settings settings)
        {
            this.settings = settings;

            var defaultProfile = GpuOverclockProfileViewModel.GetDefault(gpus);
            var profileVms = settings.Profiles.Select(p => new GpuOverclockProfileViewModel(gpus, p)).ToList();
            profileVms.Add(defaultProfile);

            Profiles = new ObservableCollection<GpuOverclockProfileViewModel>(
                profileVms.OrderBy(p => p.Name)
            );
            foreach (var vm in Profiles) vm.NameSaved += HandleProfileLabelChanged;

            Policies = new ObservableCollection<GpuOverclockPolicyViewModel>(
                settings.Policies.Select(p => new GpuOverclockPolicyViewModel(this, p))
            );
            Policies.Add(GpuOverclockPolicyViewModel.GetDefault(defaultProfile));
        }

        public ObservableCollection<GpuOverclockPolicyViewModel> Policies { get; private set; }
        public ObservableCollection<GpuOverclockProfileViewModel> Profiles { get; private set; }

        // keep profiles list sorted alphabetically
        private void HandleProfileLabelChanged(string oldLabel, string newLabel)
        {
            if (oldLabel == newLabel) return;

            var oldSorting = Profiles.Select(p => p.Name).ToList();
            var newSorting = Profiles.Select(p => p.Name).OrderBy(l => l).ToList();

            var oldIdx = oldSorting.IndexOf(newLabel);
            var newIdx = newSorting.IndexOf(newLabel);

            if (oldIdx == newIdx) return;

            Profiles.Move(oldIdx, newIdx);
        }

        public void AddPolicy(GpuOverclockPolicy policy, GpuOverclockPolicyViewModel vm)
        {
            Policies.Insert(0, vm);
            settings.Policies.Insert(0, policy);
            settings.Save();
        }

        public void AddProfile(GpuOverclockProfile profile, GpuOverclockProfileViewModel vm)
        {
            var orderedProfileNames = Profiles
                .Concat(new List<GpuOverclockProfileViewModel>() { vm })
                .Select(p => p.Name)
                .OrderBy(l => l)
                .ToList();

            var newIndex = orderedProfileNames.IndexOf(vm.Name);
            Profiles.Insert(newIndex, vm);
            settings.Profiles.Add(profile);
            settings.Save();

            vm.NameSaved += HandleProfileLabelChanged;
        }

        public void RemovePolicy(GpuOverclockPolicyViewModel policy)
        {
            Policies.Remove(policy);
            settings.Policies.Remove(policy.ModelPolicy);
            settings.Save();
        }

        public void RemoveProfile(GpuOverclockProfileViewModel profile)
        {
            Profiles.Remove(profile);
            settings.Profiles.Remove(profile.ModelProfile);

            foreach (var policy in Policies)
            {
                policy.Profiles.Remove(profile);
                policy.PendingProfiles.Remove(profile);

                policy.ModelPolicy.OrderedProfileNames.Remove(profile.Name);
            }

            settings.Save();
        }
    }
}
