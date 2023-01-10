//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------

namespace ADLXWrapper {

public class IADLXManualFanTuningState : IADLXInterface {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal IADLXManualFanTuningState(global::System.IntPtr cPtr, bool cMemoryOwn) : base(ADLXPINVOKE.IADLXManualFanTuningState_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(IADLXManualFanTuningState obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          ADLXPINVOKE.delete_IADLXManualFanTuningState(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public new static SWIGTYPE_p_wchar_t IID() {
    global::System.IntPtr cPtr = ADLXPINVOKE.IADLXManualFanTuningState_IID();
    SWIGTYPE_p_wchar_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_wchar_t(cPtr, false);
    return ret;
  }

  public virtual ADLX_RESULT GetFanSpeed(SWIGTYPE_p_int value) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXManualFanTuningState_GetFanSpeed(swigCPtr, SWIGTYPE_p_int.getCPtr(value));
    return ret;
  }

  public virtual ADLX_RESULT SetFanSpeed(int value) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXManualFanTuningState_SetFanSpeed(swigCPtr, value);
    return ret;
  }

  public virtual ADLX_RESULT GetTemperature(SWIGTYPE_p_int value) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXManualFanTuningState_GetTemperature(swigCPtr, SWIGTYPE_p_int.getCPtr(value));
    return ret;
  }

  public virtual ADLX_RESULT SetTemperature(int value) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXManualFanTuningState_SetTemperature(swigCPtr, value);
    return ret;
  }

}

}