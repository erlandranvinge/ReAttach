using System;
using System.Windows.Forms;

namespace ReAttach.Dialogs
{
    public partial class ReAttachOptionsControl : UserControl
    {
        public ReAttachOptionsPage OptionsPage { get; set; }

        public ReAttachOptionsControl()
        {
            InitializeComponent();
        }

        public void Reset()
        {
		/*
            var bus = OptionsPage.Site.GetService(typeof(IReAttachBusService)) as IReAttachBusService;
            if (bus == null) return;
            clearButton.Enabled = bus.GetReAttachHistorySize() > 0;
			*/
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
		/*
            var bus = OptionsPage.Site.GetService(typeof(IReAttachBusService)) as IReAttachBusService;
            if (bus == null) return;
            bus.ClearReAttachHistory();
            clearButton.Enabled = false;*/
        }
    }
}
