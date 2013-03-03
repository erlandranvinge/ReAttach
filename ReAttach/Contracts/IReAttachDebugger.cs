using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Data;

namespace ReAttach.Contracts
{
	public interface IReAttachDebugger : IVsDebuggerEvents, IDebugEventCallback2
	{
		bool ReAttach(ReAttachTarget target);
	}
}
