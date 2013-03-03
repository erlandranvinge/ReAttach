using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using EnvDTE80;
using EnvDTE90;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Contracts;
using ReAttach.Data;
using ReAttach.Extensions;
using ReAttach.Misc;

namespace ReAttach.Modules
{
	public class DebuggerModule : IReAttachDebugger
	{
		private readonly IReAttachPackage _package;
		private readonly IVsDebugger _debugger;
		private readonly uint _cookie;
		private readonly ReAttachTargetList _catchedTargets = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);

		public DebuggerModule(IReAttachPackage package)
		{
			_package = package;

			var serviceProvider = ModuleRepository.Resolve<IServiceContainer>();
			_debugger = serviceProvider.GetService(typeof (SVsShellDebugger)) as IVsDebugger;
			if (_debugger == null)
			{
				Trace.WriteLine("ReAttach: Can't get debugger service.");
				return;
			}

			if (_debugger.AdviseDebuggerEvents(this, out _cookie) != VSConstants.S_OK)
				Trace.TraceError("ReAttach: AdviserDebuggerEvents failed.");
		}

		public int Event(IDebugEngine2 pEngine, IDebugProcess2 pProcess, IDebugProgram2 pProgram, 
			IDebugThread2 pThread, IDebugEvent2 pEvent, ref Guid riidEvent, uint dwAttrib)
		{
			Trace.WriteLine("-DEBUGGER EVENT-: " + TypeHelper.GetType(pEvent));

			var process = (IDebugProcess3)pProcess;
			var reason = process.GetReason();
			
			if (reason != enum_DEBUG_REASON.DEBUG_REASON_USER_ATTACHED)
				return VSConstants.S_OK;

			// Remote debugging?
			IDebugCoreServer2 server;
			var serverName = string.Empty;
			if (process.GetServer(out server) == VSConstants.S_OK)
			{
				var server3 = server as IDebugCoreServer3;
				if (server3 != null && server3.QueryIsLocal() != VSConstants.S_OK)
					server3.GetMachineName(out serverName);
			}

			var pid = process.GetProcessId();
			var username = GetProcessUsername(pid);

			// TODO: Perhaps unadvise events by invoke-counting number of user attaches.
			var target = new ReAttachTarget(pid, process.GetFilename(), username, serverName);
			
			if (!_catchedTargets.Contains(target))
				_catchedTargets.AddLast(target);

			return VSConstants.S_OK;
		}

		public int OnModeChange(DBGMODE dbgmodeNew)
		{
			var ui = ModuleRepository.Resolve<UiModule>();
			switch (dbgmodeNew)
			{
				case DBGMODE.DBGMODE_Run:
					if (_debugger.AdviseDebugEventCallback(this) != VSConstants.S_OK)
						Trace.WriteLine("ReAttach: OnModeChange. AdviseDebugEventCallback failed.");
					break;
				case DBGMODE.DBGMODE_Design:
					if (_debugger.UnadviseDebugEventCallback(this) != VSConstants.S_OK)
						Trace.WriteLine("ReAttach: OnModeChange. UnadviseDebugEventCallback failed.");
					var history = ModuleRepository.Resolve<HistoryModule>();

					if (_catchedTargets.Dirty)
					{
						history.Targets.AddRangeFirst(_catchedTargets);
						history.Save();
						_catchedTargets.Clear();
					}
					ui.UpdateReAttachCommands();
					break;
			}
			return VSConstants.S_OK;
		}

		public bool TryReAttach(ReAttachTarget target)
		{
			var serviceProvider = ModuleRepository.Resolve<IServiceContainer>();
			var dte = serviceProvider.GetService(typeof(SDTE)) as DTE2;
			var debugger = dte.Debugger as Debugger2;


			List<Process3> candidates = null;
			if (!target.IsLocal)
			{
				var transport = debugger.Transports.Item("Default");
				var processes = debugger.GetProcesses(transport, target.ServerName).OfType<Process3>();
				candidates = processes.Where(p => p.Name == target.ProcessPath).ToList();
			}
			else
			{
				var processes = debugger.LocalProcesses.OfType<Process3>();
				candidates = processes.Where(p =>
					p.Name == target.ProcessPath &&
					p.UserName == target.ProcessUser).ToList();
			}

			if (!candidates.Any())
				return false;

			Process3 process = null; // First try to use the pid.
			if (target.ProcessId > 0)
				process = candidates.FirstOrDefault(p => p.ProcessID == target.ProcessId);

			// If we don't have an exact match, just go for the highest PID matching.
			if (process == null)
			{
				var maxPid = candidates.Max(p => p.ProcessID);
				process = candidates.FirstOrDefault(p => p.ProcessID == maxPid);
			}

			if (process == null)
				return false;

			try
			{
				process.Attach();
				return true;
			}
			catch
			{
			}
			return false;
		}

		private string GetProcessUsername(int pid)
		{
			var serviceProvider = ModuleRepository.Resolve<IServiceContainer>();
			var dte = serviceProvider.GetService(typeof(SDTE)) as DTE2;
			var debugger = dte.Debugger as Debugger3;
			var process = debugger.LocalProcesses.OfType<Process3>().FirstOrDefault(p => p.ProcessID == pid);
			return process != null ? process.UserName : string.Empty;
		}
	}
}
