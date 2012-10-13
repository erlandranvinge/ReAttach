// Type: EnvDTE.CommandEvents
// Assembly: EnvDTE, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\assembly\GAC\EnvDTE\8.0.0.0__b03f5f7f11d50a3a\EnvDTE.dll

using System.Runtime.InteropServices;

namespace EnvDTE
{
  [Guid("A79FC678-0D0A-496A-B9DC-0D5B9E1CA9FC")]
  [CoClass(typeof (CommandEventsClass))]
  [ComImport]
  public interface CommandEvents : _CommandEvents, _dispCommandEvents_Event
  {
  }
}
