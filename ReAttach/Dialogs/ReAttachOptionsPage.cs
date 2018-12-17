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
	        _control?.Reset();
        }

        protected override IWin32Window Window
        {
            get
            {
	            _control = new ReAttachOptionsControl {OptionsPage = this};
	            return _control;
            }
        }  
    }
}
