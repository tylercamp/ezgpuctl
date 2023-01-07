using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUControl.Lib.GPU.nvidiaimpl
{
    public class NvidiaDeviceInfo : IDeviceInfo
    {
        PhysicalGPU gpu;

        public NvidiaDeviceInfo(PhysicalGPU gpu)
        {
            this.gpu = gpu;
        }

        public string ArchitectureName => gpu.ArchitectInformation.ShortName;

        public string PciBusInfo
        {
            get
            {
                var info = gpu.BusInformation;
                return $"{info.BusType} Bus #{info.BusId}, Slot #{info.BusSlot}, x{info.CurrentPCIeLanes} mode";
            }
        }

        public string BiosVersion => gpu.Bios.VersionString;

        public int NumCores => gpu.ArchitectInformation.NumberOfCores;

        public int NumRops => gpu.ArchitectInformation.NumberOfROPs;

        // display-related queries seem to throw an exception when displays are being attached/removed

        public int NumConnectedDisplays
        {
            get
            {
                try { return gpu.ActiveOutputs.Length; }
                catch { return 0; }
            }
        }

        public int NumAvailableConnections
        {
            get
            {
                try { return gpu.GetDisplayDevices().Length; }
                catch { return 0; }
            }
        }

        public int VramSizeMB => gpu.MemoryInformation.PhysicalFrameBufferSizeInkB / 1024;
    }
}
