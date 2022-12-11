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
    public class LabeledSliderViewModel
    {
        public string Title { get; set; } = "Title";
        public decimal MinValue { get; set; } = 0;
        public decimal MaxValue { get; set; } = 100;
        public decimal CurrentValue { get; set; } = 50;

        public string Format { get; set; } = "{0}";
        public string FormattedValue => String.Format(Format, CurrentValue);
    }

    /// <summary>
    /// Interaction logic for LabeledSlider.xaml
    /// </summary>
    public partial class LabeledSlider : UserControl
    {
        public LabeledSlider()
        {
            InitializeComponent();
        }

        public string Title { get; set; } = "Title";
        public decimal MinValue { get; set; } = 0;
        public decimal MaxValue { get; set; } = 100;
        public decimal CurrentValue { get; set; } = 50;
    }
}
