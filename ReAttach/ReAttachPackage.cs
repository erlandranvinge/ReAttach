using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Services;
using ReAttach.Stores;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ReAttach
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(ReAttachConstants.ReAttachGuidString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [InstalledProductRegistration("#110", "#112", "2.4", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideOptionPage(typeof(Dialogs.ReAttachOptionsPage), "ReAttach", "General", 0, 0, true)]
    [ProvideService(typeof(ReAttachDebugger), IsAsyncQueryable = true)]
    [ProvideService(typeof(ReAttachUi), IsAsyncQueryable = true)]
    public sealed class ReAttachPackage : AsyncPackage
    {
        private ReAttachHistory _history;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            ReAttachUtils.SetUp(this);
            _history = new ReAttachHistory(this);

            AddService(typeof(ReAttachDebugger), CreateReAttachDebuggerAsync);
            AddService(typeof(ReAttachUi), CreateReAttachUiAsync);

            // This might be questionable, but time is short and initialization needed.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            _ = await GetServiceAsync(typeof(ReAttachDebugger));
            var ui = (await GetServiceAsync(typeof(ReAttachUi))) as ReAttachUi;
            ui.Update();
        }

        private async Task<object> CreateReAttachDebuggerAsync(IAsyncServiceContainer container, CancellationToken cancellationToken, Type serviceType)
        {
            var service = new ReAttachDebugger();
            await service.InitializeAsync(this, _history, cancellationToken);
            return service;
        }

        private async Task<object> CreateReAttachUiAsync(IAsyncServiceContainer container, CancellationToken cancellationToken, Type serviceType)
        {
            var service = new ReAttachUi();
            await service.InitializeAsync(this, _history, cancellationToken);
            return service;
        }
    }
}
