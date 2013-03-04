using System.Diagnostics.CodeAnalysis;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ReAttach.Misc
{
	[ExcludeFromCodeCoverage]
	internal static class DebugHelper
	{
		public static void PrintToOutputPane(string paneName, string message)
		{
			var dte = Package.GetGlobalService(typeof(SDTE)) as DTE2;
			if (dte == null)
				return;

			var w = dte.Windows.Item(EnvDTE.Constants.vsWindowKindOutput);

			w.Visible = true;
			var ow = (OutputWindow)w.Object;
			
			foreach (OutputWindowPane pane in ow.OutputWindowPanes)
			{
				if (pane.Name != paneName) continue;
				pane.OutputString(message);
				return;
			}
			var owp = ow.OutputWindowPanes.Add("Local Processes Test");
			owp.Activate();
			owp.OutputString(message);
		}

	}
}
