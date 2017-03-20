using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ReAttach.Extensions
{
	public static class DebugProcessExtensions
	{
		public static enum_DEBUG_REASON GetReason(this IDebugProcess3 process)
		{
			if (process == null)
				return enum_DEBUG_REASON.DEBUG_REASON_ERROR;

			var reason = new enum_DEBUG_REASON[1];
			if (process.GetDebugReason(reason) != VSConstants.S_OK)
				return enum_DEBUG_REASON.DEBUG_REASON_ERROR;

			return reason[0];
		}

		public static string GetFilename(this IDebugProcess2 process)
		{
			if (process == null)
				return string.Empty;
			var name = "";
			if (process.GetName(enum_GETNAME_TYPE.GN_FILENAME, out name) != VSConstants.S_OK)
				return string.Empty;
			return name;
		}

		public static string GetName(this IDebugProcess2 process)
		{
			if (process == null)
				return string.Empty;
			var name = "";
			if (process.GetName((uint)enum_GETNAME_TYPE.GN_NAME, out name) != VSConstants.S_OK)
				return string.Empty;
			return name;
		}

		public static int GetProcessId(this IDebugProcess2 process)
		{
			if (process == null)
				return 0;
			var id = new AD_PROCESS_ID[1];
			if (process.GetPhysicalProcessId(id) != VSConstants.S_OK)
				return 0;
			return (int)id[0].dwProcessId;
		}
	}
}
