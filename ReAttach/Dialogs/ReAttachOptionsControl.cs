using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Stores;
using System.Windows.Forms;

namespace ReAttach.Dialogs
{
    public partial class ReAttachOptionsControl : UserControl
    {
        private ReAttachOptionsPage _page;

        public ReAttachOptionsControl()
        {
            InitializeComponent();
        }

        public void Initialize(ReAttachOptionsPage page)
        {
            _page = page;
        }

        private void clearButton_Click(object sender, System.EventArgs e)
        {
            if (_page == null)
                return;

            var ui = _page.Site.GetService(typeof(ReAttachUi)) as ReAttachUi;
            if (ui == null)
            {
                ShowError("Failed to clear ReAttach history.", "Unable to obtain UI service.");
                return;
            }
            ui.ClearHistory();
            ShowMessage("ReAttach history cleared.");
        }

        private void ShowError(string caption, string message)
        {
            VsShellUtilities.ShowMessageBox(
                Site,
                message,
                caption,
                OLEMSGICON.OLEMSGICON_CRITICAL,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private void ShowMessage(string message)
        {
            VsShellUtilities.ShowMessageBox(
                Site,
                message,
                null,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
