using Microsoft.VisualStudio.Shell;

namespace ReAttach.Contracts
{
	public interface IReAttachPackage : IAsyncServiceContainer, IAsyncServiceProvider
	{
		IReAttachReporter Reporter { get; }
		IReAttachHistory History { get;  }
		IReAttachUi Ui { get; }
		IReAttachDebugger Debugger { get; }

		IRegistryKey OpenUserRegistryRoot();
	}
}
