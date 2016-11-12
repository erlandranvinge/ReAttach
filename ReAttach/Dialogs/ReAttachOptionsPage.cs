using System.Windows.Forms;
using System.Runtime;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace ReAttach.Dialogs
{
    public class ReAttachOptionsPage : DialogPage
    {
        protected override IWin32Window Window
        {
            get
            {
                var page = new ReAttachOptionsControl();
                page.OptionsPage = this;
                return page;
            }
        }  
    }
}
