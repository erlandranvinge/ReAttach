using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace ReAttach
{
    public static class ReAttachUtils
    {
        private static ReAttachPackage _package;
        public static void SetUp(ReAttachPackage package) => _package = package;

        public static void ShowStartupError(string message) =>
            MessageBox("ReAttach failed to start.", message, true);

        public static void ShowError(string caption, string message) =>
            MessageBox(caption, message, true);

        private static void MessageBox(string caption, string message, bool error)
        {
            VsShellUtilities.ShowMessageBox(
                _package,
                message,
                caption,
                error ? OLEMSGICON.OLEMSGICON_CRITICAL : OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
