using System.ComponentModel.Design;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace ReAttach.Tests.Misc
{
	public static class TestUtils
	{
		public static void ExecuteCommand(CommandID cmd)
		{
			object customin = null;
			object customout = null;
			var guidString = cmd.Guid.ToString("B").ToUpper();
			var cmdId = cmd.ID;
			var dte = VsIdeTestHostContext.Dte;
			dte.Commands.Raise(guidString, cmdId, ref customin, ref customout);
		}
	}
}
