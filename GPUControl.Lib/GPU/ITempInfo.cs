using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU
{
    public interface ITempInfo
    {
        decimal CurrentCoreTemp { get; }
        decimal CurrentTargetTemp { get; }

        ValueRange TargetTempRange { get; }
    }
}
