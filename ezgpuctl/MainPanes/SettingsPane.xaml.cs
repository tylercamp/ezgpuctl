using GPUControl.Controls;
using GPUControl.Lib.GPU;
using GPUControl.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GPUControl.MainPanes
{
    /// <summary>
    /// Interaction logic for PolicyServiceMainPane.xaml
    /// </summary>
    public partial class SettingsPane : UserControl
    {
        private List<IGpuWrapper> _gpus;
        private Settings _settings;
        private Action _reloadViewModel;

        public SettingsPane()
        {
            InitializeComponent();
        }

        protected MainWindowViewModel ViewModel
        {
            get => (DataContext as MainWindowViewModel)!;
            set => DataContext = value;
        }

        public void DataInit(List<IGpuWrapper> gpus, Settings settings, Action reloadViewModel)
        {
            _gpus = gpus;
            _settings = settings;
            _reloadViewModel = reloadViewModel;
        }

        private void SpecificProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.PolicyService.SetOcModeSpecificProfile.Execute(null);
        }

        private void SpecificPolicy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.PolicyService.SetOcModeSpecificPolicy.Execute(null);
        }

        private void AskBeforeClose_Clicked(object sender, RoutedEventArgs e)
        {
            _settings.AskBeforeClose = ViewModel.AskBeforeClose;
            _settings.Save();

            _reloadViewModel();
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }
    }
}
