using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADLXWrapper
{
    public static class AdlxCast
    {
        public static SWIGTYPE_p_p_adlx__IADLXManualPowerTuning ManualPowerTuningFromInterface(SWIGTYPE_p_p_adlx__IADLXInterface ptr)
        {
            var interfaceValue = ADLX.interfaceP_Ptr_value(ptr);

            var mptPtr = ADLX.new_manualPowerTuningP_Ptr();
            ADLXPINVOKE.manualFanTuningP_Ptr_assign(
                SWIGTYPE_p_p_adlx__IADLXManualPowerTuning.getCPtr(mptPtr),
                IADLXInterface.getCPtr(interfaceValue)
            );

            return mptPtr;
        }
    }
}
