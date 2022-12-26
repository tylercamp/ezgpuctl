using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GPUControl.ViewModels
{
    public partial class RangeViewModel : ObservableObject
    {
        private static readonly Regex DigitsRegex = new Regex("([\\+\\-]?\\s*[-0-9]+)");

        private string displaySuffix;
        private bool isRelative;

        public RangeViewModel()
        {
            value = 50;
            Min = 0;
            Max = 100;
            Label = "Power Target";

            displaySuffix = "%";
            isRelative = false;
        }

        public RangeViewModel(string label, ValueRange range, decimal? initialValue, string displaySuffix, bool isRelative)
        {
            Min = range.Min;
            Max = range.Max;
            Label = label;

            this.displaySuffix = displaySuffix;
            this.isRelative = isRelative;
            
            value = initialValue;
        }

        public string Label { get; }
        public decimal Min { get; }
        public decimal Max { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayValue))]
        [NotifyPropertyChangedFor(nameof(DisplayString))]
        [NotifyPropertyChangedFor(nameof(HasValue))]
        private decimal? value;

        public bool HasValue => value.HasValue;

        public decimal DisplayValue
        {
            get => value ?? 0;
            set => Value = value;
        }

        public string? DisplayString
        {
            get
            {
                if (value == null) return null;

                var formatted = isRelative ? string.Format("{0:+0;-#}", value) : value.ToString();
                return formatted + displaySuffix;
            }

            set
            {
                var str = value?.Trim() ?? "";
                if (str.Length > 0)
                {
                    var digits = DigitsRegex.Match(str).Captures.FirstOrDefault()?.Value;
                    if (digits != null)
                        Value = decimal.Parse(digits);
                }
            }
        }
    }
}
