using ReAttach.Contracts;
using ReAttach.Data;
using ReAttach.Services;
using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using VS = Microsoft.VisualStudio.Shell;

namespace ReAttach
{
	[Guid(ReAttachConstants.ReAttachPackageGuidString)]
	[VS.PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[VS.InstalledProductRegistration("#110", "#112", "2.3", IconResourceID = 400)] // Info on this package for Help/About
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	[VS.ProvideMenuResource("Menus.ctmenu", 1)]
	[VS.ProvideOptionPage(typeof(Dialogs.ReAttachOptionsPage), "ReAttach", "General", 0, 0, true)]
	[VS.ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, VS.PackageAutoLoadFlags.BackgroundLoad)]
	public sealed class ReAttachPackage : VS.AsyncPackage, IReAttachPackage
	{
		public IReAttachReporter Reporter { get; private set; }
		public IReAttachHistory History { get; private set; }
		public IReAttachUi Ui { get; private set; }
		public IReAttachDebugger Debugger { get; private set; }

		public ReAttachPackage()
		{
			Reporter = Reporter ?? new ReAttachTraceReporter();
		}

		protected override async Task InitializeAsync(System.Threading.CancellationToken cancellationToken, IProgress<VS.ServiceProgressData> progress)
		{
			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			await base.InitializeAsync(cancellationToken, progress);

			Reporter = Reporter ?? new ReAttachTraceReporter();
			History = History ?? new ReAttachHistory(new ReAttachRegistryRepository(this));
			Ui = Ui ?? await ReAttachUi.InitAsync(this);
			Debugger = Debugger ?? await ReAttachDebugger.InitAsync(this);

			History.Load();
			Ui.Update();

			var callback = new ServiceCreatorCallback(CreateBusService);
			((IServiceContainer)this).AddService(typeof(IReAttachBusService), callback);
		}

		private object CreateBusService(IServiceContainer container, Type serviceType)
		{
			return typeof(IReAttachBusService) == serviceType ? new ReAttachBusService(this) : null;
		}

		public IRegistryKey OpenUserRegistryRoot()
		{
			return new RegistryKey(UserRegistryRoot); // TODO: This needs to be handled in a different way to be testable.
		}
	}
}
