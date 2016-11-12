using System.Runtime.InteropServices;

namespace ReAttach.Contracts
{
    [Guid("214c0a97-26a2-4d93-b8ac-5de4ffff60a0")]
    [ComVisible(true)]
    public interface IReAttachBusService
    {
        void ClearReAttachHistory();
    }
}
