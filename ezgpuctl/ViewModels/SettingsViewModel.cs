using CommunityToolkit.Mvvm.ComponentModel;
using GPUControl.Lib.GPU;
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
    public partial class SettingsViewModel : ObservableObject
    {
        public SettingsViewModel()
        {
            Policies = new ObservableCollection<GpuOverclockPolicyViewModel>()
            {
                new GpuOverclockPolicyViewModel()
            };

            Profiles = new ObservableCollection<GpuOverclockProfileViewModel>()
            {
                new GpuOverclockProfileViewModel()
            };
        }

        public SettingsViewModel(List<IGpuWrapper> gpus, Settings settings)
        {
            var defaultProfile = GpuOverclockProfileViewModel.GetDefault(gpus);
            var profileVms = settings.Profiles.Select(p => new GpuOverclockProfileViewModel(gpus, p)).ToList();
            profileVms.Add(defaultProfile);

            Profiles = new ObservableCollection<GpuOverclockProfileViewModel>(
                profileVms.OrderBy(p => p.Name)
            );

            Policies = new ObservableCollection<GpuOverclockPolicyViewModel>(
                settings.Policies.Select(p => new GpuOverclockPolicyViewModel(this, p))
            );
            Policies.Add(GpuOverclockPolicyViewModel.GetDefault(defaultProfile));

            OcMode = settings.OcMode;

            if (settings.OcMode_SpecificProfileName != null)
                OcMode_SpecificProfile = Profiles.Single(p => p.Name == settings.OcMode_SpecificProfileName);

            if (settings.OcMode_SpecificPolicyName != null)
                OcMode_SpecificPolicy = Policies.Single(p => p.Name == settings.OcMode_SpecificPolicyName);
        }

        [ObservableProperty]
        private GpuOverclockProfileViewModel? ocMode_SpecificProfile;
        [ObservableProperty]
        private GpuOverclockPolicyViewModel? ocMode_SpecificPolicy;

        [ObservableProperty]
        private Settings.OcModeType ocMode = Settings.OcModeType.Policies;

        public ObservableCollection<GpuOverclockPolicyViewModel> Policies { get; private set; }
        public ObservableCollection<GpuOverclockProfileViewModel> Profiles { get; private set; }

        public Settings AsDisplayModelObject
        {
            get
            {
                var result = new Settings(null);
                result.Policies.Clear();
                result.Profiles.Clear();

                result.Policies.AddRange(Policies.Select(p => p.AsModelObject));
                result.Profiles.AddRange(Profiles.Select(p => p.AsModelObject));

                result.OcMode = OcMode;
                result.OcMode_SpecificProfileName = OcMode_SpecificProfile?.Name;
                result.OcMode_SpecificPolicyName = OcMode_SpecificPolicy?.Name;

                return result;
            }
        }
    }
}
