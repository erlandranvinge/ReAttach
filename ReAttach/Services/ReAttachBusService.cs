using System.Runtime.InteropServices;
using ReAttach.Contracts;

namespace ReAttach.Services
{
    [Guid("214c0a97-26a2-4d93-b8ac-5de4ffff60a0")]
    [ComVisible(true)]
    public interface IReAttachBusService
    {
        void ClearReAttachHistory();
        int GetReAttachHistorySize();
    }

    [Guid("37A00D3E-073E-4F5C-801F-92F763729099")]
    public class ReAttachBusService : IReAttachBusService
    {
        IReAttachPackage _package;

        public ReAttachBusService(IReAttachPackage package)
        {
            _package = package;
        }

        public void ClearReAttachHistory()
        {
            _package.History.Clear();
            _package.Ui.Update();
        }

        public int GetReAttachHistorySize()
        {
            return _package.History.Items.Count;
        }
    }
}
