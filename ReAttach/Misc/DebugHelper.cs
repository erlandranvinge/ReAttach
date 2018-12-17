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
		public static async System.Threading.Tasks.Task PrintToOutputPaneAsync(string paneName, string message)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
			var dte = Package.GetGlobalService(typeof(SDTE)) as DTE2;
			if (dte == null)
				return;

			var w = dte.Windows.Item("{34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3}");

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
