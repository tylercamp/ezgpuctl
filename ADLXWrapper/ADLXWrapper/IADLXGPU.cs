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

public class IADLXGPU : IADLXInterface {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;

  internal IADLXGPU(global::System.IntPtr cPtr, bool cMemoryOwn) : base(ADLXPINVOKE.IADLXGPU_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(IADLXGPU obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  protected override void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          ADLXPINVOKE.delete_IADLXGPU(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      base.Dispose(disposing);
    }
  }

  public new static SWIGTYPE_p_wchar_t IID() {
    global::System.IntPtr cPtr = ADLXPINVOKE.IADLXGPU_IID();
    SWIGTYPE_p_wchar_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new SWIGTYPE_p_wchar_t(cPtr, false);
    return ret;
  }

  public virtual ADLX_RESULT VendorId(SWIGTYPE_p_p_char vendorId) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_VendorId(swigCPtr, SWIGTYPE_p_p_char.getCPtr(vendorId));
    return ret;
  }

  public virtual ADLX_RESULT ASICFamilyType(SWIGTYPE_p_ADLX_ASIC_FAMILY_TYPE asicFamilyType) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_ASICFamilyType(swigCPtr, SWIGTYPE_p_ADLX_ASIC_FAMILY_TYPE.getCPtr(asicFamilyType));
    return ret;
  }

  public virtual ADLX_RESULT Type(SWIGTYPE_p_ADLX_GPU_TYPE gpuType) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_Type(swigCPtr, SWIGTYPE_p_ADLX_GPU_TYPE.getCPtr(gpuType));
    return ret;
  }

  public virtual ADLX_RESULT IsExternal(SWIGTYPE_p_bool isExternal) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_IsExternal(swigCPtr, SWIGTYPE_p_bool.getCPtr(isExternal));
    return ret;
  }

  public virtual ADLX_RESULT Name(SWIGTYPE_p_p_char name) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_Name(swigCPtr, SWIGTYPE_p_p_char.getCPtr(name));
    return ret;
  }

  public virtual ADLX_RESULT DriverPath(SWIGTYPE_p_p_char driverPath) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_DriverPath(swigCPtr, SWIGTYPE_p_p_char.getCPtr(driverPath));
    return ret;
  }

  public virtual ADLX_RESULT PNPString(SWIGTYPE_p_p_char pnpString) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_PNPString(swigCPtr, SWIGTYPE_p_p_char.getCPtr(pnpString));
    return ret;
  }

  public virtual ADLX_RESULT HasDesktops(SWIGTYPE_p_bool hasDesktops) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_HasDesktops(swigCPtr, SWIGTYPE_p_bool.getCPtr(hasDesktops));
    return ret;
  }

  public virtual ADLX_RESULT TotalVRAM(SWIGTYPE_p_unsigned_int vramMB) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_TotalVRAM(swigCPtr, SWIGTYPE_p_unsigned_int.getCPtr(vramMB));
    return ret;
  }

  public virtual ADLX_RESULT VRAMType(SWIGTYPE_p_p_char type) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_VRAMType(swigCPtr, SWIGTYPE_p_p_char.getCPtr(type));
    return ret;
  }

  public virtual ADLX_RESULT BIOSInfo(SWIGTYPE_p_p_char partNumber, SWIGTYPE_p_p_char version, SWIGTYPE_p_p_char date) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_BIOSInfo(swigCPtr, SWIGTYPE_p_p_char.getCPtr(partNumber), SWIGTYPE_p_p_char.getCPtr(version), SWIGTYPE_p_p_char.getCPtr(date));
    return ret;
  }

  public virtual ADLX_RESULT DeviceId(SWIGTYPE_p_p_char deviceId) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_DeviceId(swigCPtr, SWIGTYPE_p_p_char.getCPtr(deviceId));
    return ret;
  }

  public virtual ADLX_RESULT RevisionId(SWIGTYPE_p_p_char revisionId) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_RevisionId(swigCPtr, SWIGTYPE_p_p_char.getCPtr(revisionId));
    return ret;
  }

  public virtual ADLX_RESULT SubSystemId(SWIGTYPE_p_p_char subSystemId) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_SubSystemId(swigCPtr, SWIGTYPE_p_p_char.getCPtr(subSystemId));
    return ret;
  }

  public virtual ADLX_RESULT SubSystemVendorId(SWIGTYPE_p_p_char subSystemVendorId) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_SubSystemVendorId(swigCPtr, SWIGTYPE_p_p_char.getCPtr(subSystemVendorId));
    return ret;
  }

  public virtual ADLX_RESULT UniqueId(SWIGTYPE_p_int uniqueId) {
    ADLX_RESULT ret = (ADLX_RESULT)ADLXPINVOKE.IADLXGPU_UniqueId(swigCPtr, SWIGTYPE_p_int.getCPtr(uniqueId));
    return ret;
  }

}

}
