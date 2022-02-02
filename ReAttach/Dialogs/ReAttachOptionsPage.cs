using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ReAttach.Dialogs
{
    [Guid("E011B807-0F1F-4820-A739-9D05407FCABB")]
    public class ReAttachOptionsPage: DialogPage
    {
        protected override IWin32Window Window
        {
            get
            {
                var page = new ReAttachOptionsControl();
                page.Initialize(this);
                return page;
            }
        }
    }
}
