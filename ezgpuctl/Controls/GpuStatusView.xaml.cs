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

namespace GPUControl.Controls
{
    public class GpuStatusViewModel
    {
        public string GpuName { get; set; } = "GPU Name";
        public decimal CoreClock { get; set; } = 1200;
        public decimal CoreBaseClock { get; set; } = 1000;
        public decimal MemoryClock { get; set; } = 8000;
        public decimal MemoryBaseClock { get; set; } = 7500;

        public decimal PowerTarget { get; set; } = 100;
        public decimal CurrentPower { get; set; } = 70;
        public decimal TempTarget { get; set; } = 70;
        public decimal CurrentTemp { get; set; } = 40;

        public string CoreClockString => $"{CoreClock} MHz";
        public string CoreBaseClockString => $"{CoreBaseClock} MHz";
        public string MemoryClockString => $"{MemoryClock} MHz";
        public string MemoryBaseClockString => $"{MemoryBaseClock} MHz";
        public string PowerTargetString => $"{CurrentPower}% / {PowerTarget}%";
        public string TempTargetString => $"{CurrentTemp}C / {TempTarget}C";
    }

    /// <summary>
    /// Interaction logic for GpuStatusView.xaml
    /// </summary>
    public partial class GpuStatusView : UserControl
    {
        public GpuStatusView()
        {
            InitializeComponent();
        }
    }
}
