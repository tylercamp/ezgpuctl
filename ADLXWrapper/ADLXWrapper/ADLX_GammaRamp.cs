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

public class ADLX_GammaRamp : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ADLX_GammaRamp(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ADLX_GammaRamp obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~ADLX_GammaRamp() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          ADLXPINVOKE.delete_ADLX_GammaRamp(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public SWIGTYPE_p_unsigned_short gamma {
    set {
      ADLXPINVOKE.ADLX_GammaRamp_gamma_set(swigCPtr, SWIGTYPE_p_unsigned_short.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = ADLXPINVOKE.ADLX_GammaRamp_gamma_get(swigCPtr);
      SWIGTYPE_p_unsigned_short ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_unsigned_short(cPtr, false);
      return ret;
    } 
  }

  public ADLX_GammaRamp() : this(ADLXPINVOKE.new_ADLX_GammaRamp(), true) {
  }

}

}