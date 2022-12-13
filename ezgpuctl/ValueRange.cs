using NvAPIWrapper.GPU;
using NvAPIWrapper.Native.Interfaces.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NvAPIWrapper.Native.GPU.Structures.PerformanceStates20ParameterDelta;

namespace GPUControl
{
    public class ValueRange
    {
        public ValueRange() { Min = Max = 0; }
        public ValueRange(decimal min, decimal max) { Min = min; Max = max; }

        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Middle => (Max - Min) / 2 + Min;

        public ValueRange Map(Func<decimal, decimal> map) => new ValueRange(map(Min), map(Max));

        public override string ToString()
        {
            return $"{Min} to {Max}";
        }
    }
}
