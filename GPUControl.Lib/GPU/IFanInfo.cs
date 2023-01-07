using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU
{
    public interface IFanInfo
    {
        List<decimal> FanSpeedsPercent { get; }
        List<decimal> FanSpeedsRpm { get; }
    }
}
