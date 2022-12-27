using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.mockimpl
{
    public class MockDeviceInfo : IDeviceInfo
    {
        public string ArchitectureName => "Mock";
        public string PciBusInfo => "PCI Express Bus #0 Slot #10";

        public string BiosVersion => "100.90.80.70.60";

        public int NumCores => 4096;

        public int NumRops => 32;

        public int NumConnectedDisplays => 2;

        public int NumAvailableConnections => 12;

        public int VramSizeMB => 12000;
    }
}
