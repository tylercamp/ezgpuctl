using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU
{
    public interface IFanInfo
    {
        List<IFanInfoEntry> Entries { get; }
    }

    public interface IFanInfoEntry
    {
        int Id { get; }
        decimal FanSpeedPercent { get; }
        decimal FanSpeedRpm { get; }
    }
}
