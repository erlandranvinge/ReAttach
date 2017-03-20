using ReAttach.Contracts;
using ReAttach.Data;
using ReAttach.Services;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace ReAttach
{
	[Guid(ReAttachConstants.ReAttachPackageGuidString)]
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "2.2", IconResourceID = 400)] // Info on this package for Help/About
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideOptionPage(typeof(Dialogs.ReAttachOptionsPage), "ReAttach", "General", 0, 0, true)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	public sealed class ReAttachPackage : Package, IReAttachPackage
	{
		public IReAttachReporter Reporter { get; private set; }
		public IReAttachHistory History { get; private set; }
		public IReAttachUi Ui { get; private set; }
		public IReAttachDebugger Debugger { get; private set; }

		public ReAttachPackage()
		{
			Reporter = Reporter ?? new ReAttachTraceReporter();
		}

		protected override void Initialize()
		{
			base.Initialize();
			Reporter = Reporter ?? new ReAttachTraceReporter();
			History = History ?? new ReAttachHistory(new ReAttachRegistryRepository(this));
			Ui = Ui ?? new ReAttachUi(this);
			Debugger = Debugger ?? new ReAttachDebugger(this);

			History.Load();
			Ui.Update();

			var callback = new ServiceCreatorCallback(CreateBusService);
			((IServiceContainer)this).AddService(typeof(IReAttachBusService), callback);
		}

		private object CreateBusService(IServiceContainer container, Type serviceType)
		{
			if (typeof(IReAttachBusService) == serviceType)
				return new ReAttachBusService(this);
			return null;
		}

		public IRegistryKey OpenUserRegistryRoot()
		{
			return new RegistryKey(UserRegistryRoot); // TODO: This needs to be handled in a different way to be testable.
		}
	}
}
