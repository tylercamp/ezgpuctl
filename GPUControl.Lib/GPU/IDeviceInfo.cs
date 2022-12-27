using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU
{
    public interface IDeviceInfo
    {
        string ArchitectureName { get; }
        string PciBusInfo { get; }
        string BiosVersion { get; }
        int NumCores { get; }
        int NumRops { get; }
        int NumConnectedDisplays { get; }
        int NumAvailableConnections { get; }
        int VramSizeMB { get; }
    }
}
