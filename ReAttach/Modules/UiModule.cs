using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using ReAttach.Data;
using ReAttach.Dialogs;

namespace ReAttach.Modules
{
	public class UiModule
	{
		private readonly OleMenuCommand[] _reAttachCommands = new OleMenuCommand[ReAttachConstants.ReAttachHistorySize];
		private List<ReAttachTarget> _availableTargets = new List<ReAttachTarget>();

		public UiModule()
		{
			var serviceProvider = ModuleRepository.Resolve<IServiceContainer>();
			
			var menuService = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (menuService != null)
			{
				for (var i = 0; i < ReAttachConstants.ReAttachHistorySize; i++)
				{
					var commandId = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId + i);
					var command = new OleMenuCommand(ReAttachCommandClicked, commandId);
					//command.BeforeQueryStatus += ReAttachCommandOnBeforeQueryStatus;
					menuService.AddCommand(command);
					_reAttachCommands[i] = command;
				}
			}
		}

		public void UpdateReAttachCommands()
		{
			var history = ModuleRepository.Resolve<HistoryModule>();
			_availableTargets = history.Where(target => !target.IsAttached).ToList();

			var added = 0;
			foreach (var target in _availableTargets)
			{
				var command = _reAttachCommands[added];
				command.Text = "ReAttach to " + target;
				command.Visible = true;
				command.Enabled = true;
				added++;
			}

			for (var i = added; i < ReAttachConstants.ReAttachHistorySize; i++)
			{
				var command = _reAttachCommands[i];
				command.Visible = false;
				command.Enabled = false;
			}
		}


		/*
		public void EnableReAttachCommands(bool enabled)
		{
			for (var i = 0; i < ReAttachConstants.ReAttachHistorySize; i++)
			{
				_reAttachCommands[i].Enabled = enabled;
				_reAttachCommands[i].Visible = enabled;
			}
		}

		public void EnableReAttachCommand(int index, bool enabled)
		{
			if (index < 0 || index >= ReAttachConstants.ReAttachHistorySize)
				return;
			_reAttachCommands[index].Enabled = enabled;
			_reAttachCommands[index].Visible = enabled;
		}
		*/

		private void ReAttachCommandOnBeforeQueryStatus(object sender, EventArgs eventArgs)
		{
			var command = sender as OleMenuCommand;
			if (command == null)
				return;

			var history = ModuleRepository.Resolve<HistoryModule>();
			var index = command.CommandID.ID - ReAttachConstants.ReAttachCommandId;

			var target = history.Targets[index];
			command.Enabled = target != null;
			command.Visible = target != null;
		}

		private void ReAttachCommandClicked(object sender, EventArgs e)
		{
			var command = sender as OleMenuCommand;
			if (command == null)
				return;

			var history = ModuleRepository.Resolve<HistoryModule>();
			var index = command.CommandID.ID - ReAttachConstants.ReAttachCommandId;

			if (index >= _availableTargets.Count)
				return;

			var target = _availableTargets[index];
			if (target == null)
				return;

			var debuggerModule = ModuleRepository.Resolve<DebuggerModule>();
			if (debuggerModule.TryReAttach(target) || new ReAttachDialog(target).ShowModal() == true)
			{
				target.IsAttached = true;
				UpdateReAttachCommands();
			}
		}
	}
}
