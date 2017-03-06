using System.Windows.Forms;
using System.Runtime;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace ReAttach.Dialogs
{
    public class ReAttachOptionsPage : DialogPage
    {
        private ReAttachOptionsControl _control;

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);
            if (_control != null) _control.Reset();
        }

        protected override IWin32Window Window
        {
            get
            {
                _control = new ReAttachOptionsControl();
                _control.OptionsPage = this;
                return _control;
            }
        }  
    }
}
