using System;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Contracts;
using ReAttach.Data;

namespace ReAttach
{
	[Guid(ReAttachConstants.ReAttachPackageGuidString)]
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.1", IconResourceID = 400)] // Needed to show this package in help /about.
	[ProvideMenuResource("Menus.ctmenu", 1)] // Required to show menus.
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	public sealed class ReAttachPackage : Package, IReAttachPackage
	{
		public IReAttachReporter Reporter { get; private set; }
		public IReAttachHistory History { get; private set; }
		public IReAttachUi Ui { get; private set; }
		public IReAttachDebugger Debugger { get; private set; }

		public ReAttachPackage() {}

		public ReAttachPackage(IReAttachReporter reporter, IReAttachHistory history, IReAttachUi ui, IReAttachDebugger debugger)
		{
			Reporter = reporter;
			History = history;
			Ui = ui;
			Debugger = debugger;
		}

		protected override void Initialize()
		{
			base.Initialize();

			// Wire-up modules. No XML-based unity.config from hell thank you.
			Reporter = Reporter ?? new ReAttachTraceReporter();
			History =  History ?? new ReAttachHistory(new ReAttachRegistryRepository(this));
			Ui = Ui ?? new ReAttachUi(this);
			Debugger = Debugger ?? new ReAttachDebugger(this);

			History.Load();
			Ui.Update();
		}

		public IRegistryKey OpenUserRegistryRoot()
		{
			return new RegistryKey(UserRegistryRoot); // TODO: This needs to be handled in a different way to be testable.
		}
	}
}
