using System.ComponentModel.Design;
using Microsoft.VisualStudio.OLE.Interop;
using ReAttach.Wrappers;

namespace ReAttach.Contracts
{
	public interface IReAttachPackage : IServiceContainer, IServiceProvider
	{
		IReAttachReporter Reporter { get; }
		IReAttachHistory History { get;  }
		IReAttachUi Ui { get; }
		IReAttachDebugger Debugger { get; }

		IRegistryKey OpenUserRegistryRoot();
	}
}
