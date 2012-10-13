// Type: EnvDTE._dispCommandEvents_Event
// Assembly: EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\assembly\GAC\EnvDTE\8.0.0.0__b03f5f7f11d50a3a\EnvDTE.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EnvDTE
{
  [ComVisible(false)]
  [TypeLibType((short) 16)]
  [ComEventInterface(typeof (_dispCommandEvents\u0000), typeof (_dispCommandEvents_EventProvider\u0000))]
  public interface _dispCommandEvents_Event
  {
    event _dispCommandEvents_BeforeExecuteEventHandler BeforeExecute;

    event _dispCommandEvents_AfterExecuteEventHandler AfterExecute;

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void add_BeforeExecute(_dispCommandEvents_BeforeExecuteEventHandler A_1);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void remove_BeforeExecute(_dispCommandEvents_BeforeExecuteEventHandler A_1);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void add_AfterExecute(_dispCommandEvents_AfterExecuteEventHandler A_1);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void remove_AfterExecute(_dispCommandEvents_AfterExecuteEventHandler A_1);
  }
}
