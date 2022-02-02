using ReAttach.Models;
using System.Windows;

namespace ReAttach.Dialogs
{
    public partial class ReAttachDialog
    {
        public ReAttachDialog(ReAttachPackage package, ReAttachTarget target)
        {
            InitializeComponent();
        }
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
