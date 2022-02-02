using EnvDTE90;
using ReAttach.Models;
using System;

namespace ReAttach.Extensions
{
	public static class ProcessExtensions
	{
		public static string GetUsername(this Process3 process)
		{
			var name = process.UserName;
			if (string.IsNullOrEmpty(name))
				return name;
			var start = name.LastIndexOf('[');
			return start != -1 ? name.Substring(0, start).TrimEnd() : name;
		}

		public static bool IsMatchingLocalProcess(this Process3 process, ReAttachTarget target)
        {
			return
				string.Compare(process.Name, target.ProcessPath, StringComparison.OrdinalIgnoreCase) == 0 &&
				string.Compare(process.UserName, target.ProcessUser) == 0;
		}

		public static bool IsMatchingExclusively(this Process3 process, ReAttachTarget target)
        {
			if (!string.IsNullOrEmpty(process.UserName))
				return false;

			return string.Compare(process.Name, target.ProcessName, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public static bool IsMatchingRemoteProcess(this Process3 process, ReAttachTarget target)
        {
			return string.Compare(process.Name, target.ProcessPath, StringComparison.OrdinalIgnoreCase) == 0;
		}

	}
}
