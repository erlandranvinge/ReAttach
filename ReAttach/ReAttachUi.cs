using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Contracts;
using ReAttach.Dialogs;
using EnvDTE80;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace ReAttach
{
	public class ReAttachUi : IReAttachUi
	{
		private readonly IReAttachPackage _package;
		private readonly OleMenuCommand _buildToggleCommand;
		public readonly OleMenuCommand[] Commands = new OleMenuCommand[ReAttachConstants.ReAttachHistorySize];

		private ReAttachUi(IReAttachPackage package, IMenuCommandService menuService)
		{
			_package = package;
			for (var i = 0; i < ReAttachConstants.ReAttachHistorySize; i++)
			{
				var commandId = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId + i);
				var command = new OleMenuCommand(async (s, e) => await ReAttachCommandClickedAsync(s, e), commandId);
				//command.BeforeQueryStatus += ReAttachCommandOnBeforeQueryStatus;
				menuService.AddCommand(command);

				if (i > 0) // Hide all items except first one initially.
					command.Visible = false;

				Commands[i] = command;
			}

			var buildToggleCommandId = new CommandID(ReAttachConstants.ReAttachPackageCmdSet,
				ReAttachConstants.BuildBeforeReAttachCommandId);
			var buildCommand = new OleMenuCommand(ReAttachToggleBuildClicked, buildToggleCommandId);
			buildCommand.Visible = true;
			buildCommand.Checked = _package.History.Options.BuildBeforeReAttach;
			menuService.AddCommand(buildCommand);
			_buildToggleCommand = buildCommand;
		}

		public static async Task<ReAttachUi> InitAsync(IReAttachPackage package)
		{
		
			var menuService = await package.GetServiceAsync(typeof(IMenuCommandService)) as IMenuCommandService;
			if (menuService == null)
			{
				package.Reporter.ReportError("Can\'t obtain a reference to IMenuCommandService from ReAttachUI ctor.");
				return null;
			}
			return new ReAttachUi(package, menuService);
		}

		public void Update()
		{
			// Hide all items in menus that doesn't have a corresponding target.
			var nonAttachedItems = _package.History.Items.Where(t => !t.IsAttached).ToList();
			if (!nonAttachedItems.Any())
			{
				Commands[0].Text = ReAttachConstants.Texts.NoTargetsAvailable;
				Commands[0].Visible = true;
				Commands[0].Enabled = false;

				for (var i = 1; i < ReAttachConstants.ReAttachHistorySize; i++)
					Commands[i].Enabled = Commands[i].Visible = false;

				return;
			}

			for (var i = 0; i < ReAttachConstants.ReAttachHistorySize; i++)
			{
				var item = i < nonAttachedItems.Count ? nonAttachedItems[i] : null;
				if (item != null)
				{
					Commands[i].Text = ReAttachConstants.Texts.MenuItemPrefix + item;
					Commands[i].Visible = true;
					Commands[i].Enabled = true;
				}
				else
				{
					Commands[i].Visible = false;
					Commands[i].Enabled = false;
				}
			}
		}

		public async Task ReAttachCommandClickedAsync(object sender, EventArgs e)
		{
			var command = sender as OleMenuCommand;
			if (command == null)
			{
				_package.Reporter.ReportWarning("ReAttachCommandClick sent from non-OleMenuCommand.");
				return;
			}
			var index = command.CommandID.ID - ReAttachConstants.ReAttachCommandId;

			var unAttachedTargets = _package.History.Items.Where(t => !t.IsAttached).ToList();
			var target = index < unAttachedTargets.Count ? unAttachedTargets[index] : null;
			if (target == null)
				return;

			if (_package.History.Options.BuildBeforeReAttach)
				await TryBuildSolutionAsync();

			if (!_package.Debugger.ReAttach(target))
			{
				var dialog = new ReAttachDialog(_package, target);
				dialog.ShowModal();
			}
		}

		public void ReAttachToggleBuildClicked(object sender, EventArgs e)
		{
			var toggle = !_package.History.Options.BuildBeforeReAttach;
			_package.History.Options.BuildBeforeReAttach = toggle;
			_buildToggleCommand.Checked = toggle;
		}

		public async Task TryBuildSolutionAsync()
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
			try
			{
				var dte = await _package.GetServiceAsync(typeof(SDTE)) as DTE2;
				if (dte == null) throw new ArgumentNullException(nameof(dte));

				dte.Solution.SolutionBuild.Build(true);
			}
			catch (Exception) { }
		}

		public async Task MessageBoxAsync(string message)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
			var uiShell = await _package.GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
			if (uiShell == null)
				return;

			var clsid = Guid.Empty;
			uiShell.ShowMessageBox(
				0,
				ref clsid,
				"ReAttach",
				message,
				string.Empty,
				0,
				OLEMSGBUTTON.OLEMSGBUTTON_OK,
				OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
				OLEMSGICON.OLEMSGICON_INFO,
				0, // false
				out int result);
		}
	}
}
