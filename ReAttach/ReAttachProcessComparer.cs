using EnvDTE90;
using ReAttach.Data;
using System;

namespace ReAttach
{
	public static class ReAttachProcessComparer
	{
		public static bool CompareRemoteProcess(Process3 process, ReAttachTarget target)
		{
			return string.Compare(process.Name, target.ProcessPath, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool CompareProcess(Process3 process, ReAttachTarget target)
		{
			return
				string.Compare(process.Name, target.ProcessPath, StringComparison.OrdinalIgnoreCase) == 0 &&
				string.Compare(process.UserName, target.ProcessUser) == 0;
		}

		public static bool CompareExclusiveProcess(Process3 process, ReAttachTarget target)
		{
			if (!string.IsNullOrEmpty(process.UserName))
				return false;

			return string.Compare(process.Name, target.ProcessName, StringComparison.OrdinalIgnoreCase) == 0;
				
		}
		
	}
}
