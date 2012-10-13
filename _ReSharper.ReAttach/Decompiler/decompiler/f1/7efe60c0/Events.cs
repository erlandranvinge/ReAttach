// Type: EnvDTE.Events
// Assembly: EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\assembly\GAC\EnvDTE\8.0.0.0__b03f5f7f11d50a3a\EnvDTE.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EnvDTE
{
  [TypeLibType((short) 4160)]
  [Guid("134170F8-93B1-42DD-9F89-A2AC7010BA07")]
  [ComImport]
  public interface Events
  {
    [DispId(301)]
    SelectionEvents SelectionEvents { [DispId(301), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(302)]
    SolutionEvents SolutionEvents { [DispId(302), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(303)]
    BuildEvents BuildEvents { [DispId(303), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(306)]
    FindEvents FindEvents { [DispId(306), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(308)]
    DTEEvents DTEEvents { [DispId(308), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(310)]
    ProjectItemsEvents SolutionItemsEvents { [DispId(310), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(311)]
    ProjectItemsEvents MiscFilesEvents { [DispId(311), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(312)]
    DebuggerEvents DebuggerEvents { [DispId(312), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(205)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.IDispatch)]
    object get_CommandBarEvents([MarshalAs(UnmanagedType.IDispatch), In] object CommandBarControl);

    [DispId(300)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    CommandEvents get_CommandEvents([MarshalAs(UnmanagedType.BStr), In] string Guid = "{00000000-0000-0000-0000-000000000000}", [In] int ID = 0);

    [DispId(304)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    WindowEvents get_WindowEvents([MarshalAs(UnmanagedType.Interface), In] Window WindowFilter = null);

    [DispId(305)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    OutputWindowEvents get_OutputWindowEvents([MarshalAs(UnmanagedType.BStr), In] string Pane = "");

    [DispId(307)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    TaskListEvents get_TaskListEvents([MarshalAs(UnmanagedType.BStr), In] string Filter = "");

    [DispId(309)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    DocumentEvents get_DocumentEvents([MarshalAs(UnmanagedType.Interface), In] Document Document = null);

    [DispId(313)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    TextEditorEvents get_TextEditorEvents([MarshalAs(UnmanagedType.Interface), In] TextDocument TextDocumentFilter = null);

    [DispId(314)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.IDispatch)]
    object GetObject([MarshalAs(UnmanagedType.BStr), In] string Name);
  }
}
