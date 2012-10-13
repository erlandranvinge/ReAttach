using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using ReAttach.Dialogs;

namespace ReAttach.Modules
{
	public class UiModule
	{
		private readonly OleMenuCommand[] _reAttachHistoryCommands = new OleMenuCommand[Constants.ReAttachHistorySize];

		public UiModule()
		{
			var serviceProvider = ModuleRepository.Resolve<IServiceContainer>();
			
			var menuService = serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (menuService != null)
			{
				for (var i = 0; i < Constants.ReAttachHistorySize; i++)
				{
					var commandId = new CommandID(Constants.ReAttachPackageCmdSet, Constants.ReAttachCommandId + i);
					var command = new OleMenuCommand(ReAttachCommandClicked, commandId);
					command.BeforeQueryStatus += ReAttachCommandOnBeforeQueryStatus;
					menuService.AddCommand(command);
					_reAttachHistoryCommands[i] = command;
				}
			}
		}

		public void Update()
		{
			var history = ModuleRepository.Resolve<ReAttachTargets>();
			int i = 0;
			foreach (var item in history.Items)
			{
				_reAttachHistoryCommands[i++].Text = string.Format("ReAttach to {0} ({1})",
					item.ProcessName, item.ProcessUser);
			}
		}

		public void Enable(bool enabled)
		{
			for (int i = 0; i < Constants.ReAttachHistorySize; i++)
			{
				_reAttachHistoryCommands[i].Enabled = enabled;
				_reAttachHistoryCommands[i].Visible = enabled;
			}
		}

		private void ReAttachCommandOnBeforeQueryStatus(object sender, EventArgs eventArgs)
		{
			var command = sender as OleMenuCommand;
			if (command == null)
				return;

			var history = ModuleRepository.Resolve<ReAttachTargets>();

			var index = command.CommandID.ID - Constants.ReAttachCommandId;
			var target = history.TryGetItem(index);
			command.Visible = target != null;
		}

		private void ReAttachCommandClicked(object sender, EventArgs e)
		{
			var command = sender as OleMenuCommand;
			if (command == null)
				return;

			var history = ModuleRepository.Resolve<ReAttachTargets>();
			var index = command.CommandID.ID - Constants.ReAttachCommandId;

			var target = history.TryGetItem(index);
			if (target == null)
				return;

			var debuggerModule = ModuleRepository.Resolve<DebuggerModule>();
			if (!debuggerModule.TryReAttach(target))
			{
				var attachDialog = new ReAttachDialog(target);
				attachDialog.ShowModal();
			}
		}
	}
}
