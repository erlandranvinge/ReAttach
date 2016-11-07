using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Contracts;
using ReAttach.Dialogs;

namespace ReAttach
{
	public class ReAttachUi : IReAttachUi
	{
		private readonly IReAttachPackage _package;
		public readonly OleMenuCommand[] Commands = new OleMenuCommand[ReAttachConstants.ReAttachHistorySize];
        public readonly OleMenuCommand ReBuildBeforeReattachCommand;

		public ReAttachUi(IReAttachPackage package)
		{
			_package = package;
			var menuService = _package.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
			if (menuService == null)
			{
				_package.Reporter.ReportError("Can\'t obtain a reference to IMenuCommandService from ReAttachUI ctor.");
				return;
			}

			for (var i = 0; i < ReAttachConstants.ReAttachHistorySize; i++)
			{
				var commandId = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId + i);
				var command = new OleMenuCommand(ReAttachCommandClicked, commandId);
				//command.BeforeQueryStatus += ReAttachCommandOnBeforeQueryStatus;
				menuService.AddCommand(command);

				if (i > 0) // Hide all items except first one initially.
					command.Visible = false;
	
				Commands[i] = command;
			}

            var rebuildCommandId = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, 
                ReAttachConstants.RebuildBeforeReAttachCommandId);
            var rebuildCommand = new OleMenuCommand(ReAttachToggleReBuildClicked, rebuildCommandId);
            rebuildCommand.Visible = true;
            rebuildCommand.Enabled = true;
            rebuildCommand.Checked = true;
            menuService.AddCommand(rebuildCommand);
            ReBuildBeforeReattachCommand = rebuildCommand;
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

		public void ReAttachCommandClicked(object sender, EventArgs e)
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

			if (!_package.Debugger.ReAttach(target))
			{
				var dialog = new ReAttachDialog(_package, target);
				dialog.ShowModal();
			}
		}

        public void ReAttachToggleReBuildClicked(object sender, EventArgs e)
        {

        }

		public void MessageBox(string message)
		{
			var uiShell = (IVsUIShell)_package.GetService(typeof(SVsUIShell));
			var clsid = Guid.Empty;
			int result;
			
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
				out result);
		}
	}
}
