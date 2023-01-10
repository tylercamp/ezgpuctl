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

public class IADLXSystemMetrics : IADLXInterface {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal IADLXSystemMetrics(global::System.IntPtr cPtr, bool cMemoryOwn) : base(ADLXPINVOKE.IADLXSystemMetrics_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(IADLXSystemMetrics obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          ADLXPINVOKE.delete_IADLXSystemMetrics(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public new static SWIGTYPE_p_wchar_t IID() {
    global::System.IntPtr cPtr = ADLXPINVOKE.IADLXSystemMetrics_IID();
    SWIGTYPE_p_wchar_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_wchar_t(cPtr, false);
    return ret;
  }

  public virtual ADLX_RESULT TimeStamp(SWIGTYPE_p_long_long ms) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXSystemMetrics_TimeStamp(swigCPtr, SWIGTYPE_p_long_long.getCPtr(ms));
    return ret;
  }

  public virtual ADLX_RESULT CPUUsage(SWIGTYPE_p_double data) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXSystemMetrics_CPUUsage(swigCPtr, SWIGTYPE_p_double.getCPtr(data));
    return ret;
  }

  public virtual ADLX_RESULT SystemRAM(SWIGTYPE_p_int data) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXSystemMetrics_SystemRAM(swigCPtr, SWIGTYPE_p_int.getCPtr(data));
    return ret;
  }

  public virtual ADLX_RESULT SmartShift(SWIGTYPE_p_int data) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXSystemMetrics_SmartShift(swigCPtr, SWIGTYPE_p_int.getCPtr(data));
    return ret;
  }

}

}
