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

public class ADLX_RegammaCoeff : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal ADLX_RegammaCoeff(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(ADLX_RegammaCoeff obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~ADLX_RegammaCoeff() {
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
          ADLXPINVOKE.delete_ADLX_RegammaCoeff(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

  public int coefficientA0 {
    set {
      ADLXPINVOKE.ADLX_RegammaCoeff_coefficientA0_set(swigCPtr, value);
    } 
    get {
      int ret = ADLXPINVOKE.ADLX_RegammaCoeff_coefficientA0_get(swigCPtr);
      return ret;
    } 
  }

  public int coefficientA1 {
    set {
      ADLXPINVOKE.ADLX_RegammaCoeff_coefficientA1_set(swigCPtr, value);
    } 
    get {
      int ret = ADLXPINVOKE.ADLX_RegammaCoeff_coefficientA1_get(swigCPtr);
      return ret;
    } 
  }

  public int coefficientA2 {
    set {
      ADLXPINVOKE.ADLX_RegammaCoeff_coefficientA2_set(swigCPtr, value);
    } 
    get {
      int ret = ADLXPINVOKE.ADLX_RegammaCoeff_coefficientA2_get(swigCPtr);
      return ret;
    } 
  }

  public int coefficientA3 {
    set {
      ADLXPINVOKE.ADLX_RegammaCoeff_coefficientA3_set(swigCPtr, value);
    } 
    get {
      int ret = ADLXPINVOKE.ADLX_RegammaCoeff_coefficientA3_get(swigCPtr);
      return ret;
    } 
  }

  public int gamma {
    set {
      ADLXPINVOKE.ADLX_RegammaCoeff_gamma_set(swigCPtr, value);
    } 
    get {
      int ret = ADLXPINVOKE.ADLX_RegammaCoeff_gamma_get(swigCPtr);
      return ret;
    } 
  }

  public ADLX_RegammaCoeff() : this(ADLXPINVOKE.new_ADLX_RegammaCoeff(), true) {
  }

}

}