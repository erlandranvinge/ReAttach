using System.Runtime.InteropServices;
using ReAttach.Contracts;

namespace ReAttach
{
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
    }
}
