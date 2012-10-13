using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using ReAttach.Modules;

namespace ReAttach
{
	[Guid(Constants.ReAttachPackageGuidString)]
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Needed to show this package in help /about.
    [ProvideMenuResource("Menus.ctmenu", 1)] // Required to show menus.
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	public sealed class ReAttachPackage : Package
    {
		protected override void Initialize()
		{
			base.Initialize();
			ModuleRepository.Register(this);
			ModuleRepository.Register<IServiceContainer>(this);
			ModuleRepository.Register(new UiModule());
			ModuleRepository.Register(new DebuggerModule());
			ModuleRepository.Register(new ReAttachTargets());

			LoadSettings();
		}

		private void LoadSettings()
		{
			var history = ModuleRepository.Resolve<ReAttachTargets>();
			history.Load();
			var ui = ModuleRepository.Resolve<UiModule>();
			ui.Update();
		}
    }
}
