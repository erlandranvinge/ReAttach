using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Dialogs;
using ReAttach.Services;
using ReAttach.Stores;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReAttach
{
    public class ReAttachUi
    {
        private ReAttachPackage _package;
        private ReAttachHistory _history;
        private ReAttachOptions _options;

        private IMenuCommandService _menu;
        private OleMenuCommand[] _commands = new OleMenuCommand[ReAttachConstants.ReAttachHistorySize];
        private OleMenuCommand _buildToggleCommand;

        public async Task InitializeAsync(ReAttachPackage package, ReAttachHistory history, CancellationToken cancellationToken)
        {
            _package = package;
            _history = history;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            _menu = (await package.GetServiceAsync(typeof(IMenuCommandService))) as IMenuCommandService;
            if (_menu == null)
            {
                ReAttachUtils.ShowStartupError("Unable to obtain reference to menu service.");
                return;
            }

            _options = new ReAttachOptions(package);

            for (var i = 0; i < ReAttachConstants.ReAttachHistorySize; i++)
            {
                var commandId = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId + i);
                var command = new OleMenuCommand(ReAttachCommandClicked, commandId);
                _menu.AddCommand(command);

                if (i > 0) // Hide all items except first one initially.
                    command.Visible = false;

                _commands[i] = command;
            }

            var buildToggleCommandId = new CommandID(ReAttachConstants.ReAttachPackageCmdSet,
                ReAttachConstants.BuildBeforeReAttachCommandId);
            var buildCommand = new OleMenuCommand(ReAttachToggleBuildClicked, buildToggleCommandId);
            buildCommand.Visible = true;
            buildCommand.Checked = _options.BuildBeforeReAttach;
            _menu.AddCommand(buildCommand);
            _buildToggleCommand = buildCommand;
        }

        public void Update()
        {
            // Hide all items in menus that doesn't have a corresponding target.
            var nonAttachedItems = _history.GetUnAttached();
            if (!nonAttachedItems.Any())
            {
                _commands[0].Text = ReAttachConstants.Texts.NoTargetsAvailable;
                _commands[0].Visible = true;
                _commands[0].Enabled = false;

                for (var i = 1; i < ReAttachConstants.ReAttachHistorySize; i++)
                    _commands[i].Enabled = _commands[i].Visible = false;

                return;
            }

            for (var i = 0; i < ReAttachConstants.ReAttachHistorySize; i++)
            {
                var item = i < nonAttachedItems.Length ? nonAttachedItems[i] : null;
                if (item != null)
                {
                    _commands[i].Text = ReAttachConstants.Texts.MenuItemPrefix + item;
                    _commands[i].Visible = true;
                    _commands[i].Enabled = true;
                }
                else
                {
                    _commands[i].Visible = false;
                    _commands[i].Enabled = false;
                }
            }
        }

        private void ReAttachCommandClicked(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var command = sender as OleMenuCommand;
                if (command == null)
                {
                    ReAttachUtils.ShowError("ReAttach failed.", "Click was sent from non-ole command.");
                    return;
                }

                var index = command.CommandID.ID - ReAttachConstants.ReAttachCommandId;
                var unAttachedTargets = _history.GetUnAttached();
                var target = index < unAttachedTargets.Length ? unAttachedTargets[index] : null;
                if (target == null)
                    return;

                if (_options.BuildBeforeReAttach)
                    await TryBuildSolutionAsync();

                var debugger = (await _package.GetServiceAsync(typeof(ReAttachDebugger))) as ReAttachDebugger;
                if (debugger == null)
                {
                    ReAttachUtils.ShowError("ReAttach failed.", "Unable to obtain ref to debugger");
                    return;
                }

                if (!debugger.ReAttach(target))
                {
                    var dialog = new WaitingDialog(debugger, target);
                    dialog.ShowModal();
                }
            }).FileAndForget("vs/reattach/reattach");
        }

        public void ReAttachToggleBuildClicked(object sender, EventArgs e)
        {
            _options.BuildBeforeReAttach = !_options.BuildBeforeReAttach;
            _buildToggleCommand.Checked = _options.BuildBeforeReAttach;
        }

        public void ClearHistory()
        {
            _history.Clear();
            Update();
        }
        private async Task TryBuildSolutionAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                var dte = await _package.GetServiceAsync(typeof(SDTE)) as DTE2;
                if (dte == null)
                {
                    ReAttachUtils.ShowError("ReAttach failed", "Unable to rebuild solution before build.");
                    return;
                }
                dte.Solution.SolutionBuild.Build(true);
            }
            catch (Exception) { }
        }
    }
}
