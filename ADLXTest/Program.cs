using ADLXWrapper;

var helper = new ADLXHelper();
try
{
    if (helper.Initialize() != ADLX_RESULT.ADLX_OK) throw new Exception("ADLX init failed");
    IADLXSystem sys = helper.GetSystemServices();

    var glPtr = ADLX.new_gpuListP_Ptr();
    if (sys.GetGPUs(glPtr) != ADLX_RESULT.ADLX_OK) throw new Exception("GetGPUs failed");
    var gpuList = ADLX.gpuListP_Ptr_value(glPtr);

    Console.WriteLine("Got {0} GPUs", gpuList.Size());
    if (gpuList.Size() == 0) throw new Exception("No GPUs available");

    var gPtr = ADLX.new_gpuP_Ptr();
    if (gpuList.At(0, gPtr) != ADLX_RESULT.ADLX_OK) throw new Exception("gpuList.At failed");
    var gpu = ADLX.gpuP_Ptr_value(gPtr);
    
    var gtsPtr = ADLX.new_gpuTuningServicesP_Ptr();
    if (sys.GetGPUTuningServices(gtsPtr) != ADLX_RESULT.ADLX_OK) throw new Exception("GetGPUTuningServices failed");
    var tuningServices = ADLX.gpuTuningServicesP_Ptr_value(gtsPtr);

    var bPtr = ADLX.new_boolP();
    if (tuningServices.IsSupportedManualPowerTuning(gpu, bPtr) != ADLX_RESULT.ADLX_OK) throw new Exception("IsSupportedManualPowerTuning failed");
    var powerTuningSupported = ADLX.boolP_value(bPtr);
    if (!powerTuningSupported) throw new Exception("Power tuning not supported");
    else Console.WriteLine("Power tuning supported");

    var mptiPtr = ADLX.new_interfaceP_Ptr();
    tuningServices.GetManualPowerTuning(gpu, mptiPtr);
    var mptPtr = AdlxCast.ManualPowerTuningFromInterface(mptiPtr);
    var powerTuning = ADLX.manualPowerTuningP_Ptr_value(mptPtr);

    // doesn't work (?)
    var intPtr = ADLX.new_intP();
    if (powerTuning.GetPowerLimit(intPtr) != ADLX_RESULT.ADLX_OK) throw new Exception("GetPowerLimit failed");
    Console.WriteLine("Power limit: {0}", ADLX.intP_value(intPtr));

    //if (powerTuning.GetTDCLimit(intPtr) != ADLX_RESULT.ADLX_OK) throw new Exception("GetTDCLimit failed");
    Console.WriteLine("TDC limit: {0}", ADLX.intP_value(intPtr));

    var irangePtr = ADLX.new_intRangeTypeP();
    if (powerTuning.GetPowerLimitRange(irangePtr) != ADLX_RESULT.ADLX_OK) throw new Exception("GetPowerLimitRange failed");
    Console.WriteLine("{0} - {1}", irangePtr.minValue, irangePtr.maxValue);
}
finally
{
    helper.Terminate();
}

Console.ReadLine();