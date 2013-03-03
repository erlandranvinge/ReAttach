using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Contracts;
using ReAttach.Dialogs;
using ReAttach.Misc;

namespace ReAttach
{
	public class ReAttachUi : IReAttachUi
	{
		private readonly IReAttachPackage _package;
		private readonly OleMenuCommand[] _commands = new OleMenuCommand[ReAttachConstants.ReAttachHistorySize];

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
	
				_commands[i] = command;
			}
		}

		public void Update()
		{
			// If no targets are loaded, just show a default button.
			if (_package.History.Items.IsEmpty) 
			{
				_commands[0].Text = "ReAttach...";
				_commands[0].Visible = true;
				for (var i = 1; i < ReAttachConstants.ReAttachHistorySize; i++)
					_commands[i].Visible = false;
				return;
			}

			// Hide all items in menus that doesn't have a corresponding target.
			var items = _package.History.Items.Where(t => !t.IsAttached).ToList();
			for (var i = 0; i < ReAttachConstants.ReAttachHistorySize; i++)
			{
				var item = i < items.Count ? items[i] : null;
				if (item != null)
				{
					_commands[i].Text = item.ToString();
					_commands[i].Visible = true;
				}
				else
				{
					_commands[i].Visible = false;
				}
			}
		}

		public void ReAttachCommandClicked(object sender, EventArgs e)
		{
			/*
			const int E_ELEVATION_REQUIRED = unchecked((int)0x800702E4);
			Marshal.ThrowExceptionForHR(E_ELEVATION_REQUIRED); // Show UAC box.
			return;
			*/
			if (_package.History.Items.IsEmpty)
			{
				ExecuteCommand("Debug.AttachToProcess");
				return;
			}

			var command = sender as OleMenuCommand;
			if (command == null)
			{
				_package.Reporter.ReportWarning("ReAttachCommandClick sent from non OleMenuCommand.");
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

		private void ExecuteCommand(string command, string args = "")
		{
			var dte = _package.GetService(typeof(SDTE)) as DTE2;
			if (dte == null)
			{
				_package.Reporter.ReportError("Unable to get instance of SDTE/DTE2 from ExecuteCommand method.");
				return;
			}
			dte.ExecuteCommand(command);
		}
	}
}
