using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using EnvDTE80;
using EnvDTE90;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Extensions;

namespace ReAttach.Modules
{
	public class DebuggerModule : IDebugEventCallback2, IVsDebuggerEvents
	{
		private readonly IVsDebugger _debugger;
		private readonly uint _cookie;

		public DebuggerModule()
		{
			var serviceProvider = ModuleRepository.Resolve<IServiceContainer>();
			_debugger = serviceProvider.GetService(typeof (SVsShellDebugger)) as IVsDebugger;
			if (_debugger.AdviseDebuggerEvents(this, out _cookie) != VSConstants.S_OK)
				Trace.TraceError("ReAttach: AdviserDebuggerEvents failed.");
		}

		public int Event(IDebugEngine2 pEngine, IDebugProcess2 pProcess, IDebugProgram2 pProgram, 
			IDebugThread2 pThread, IDebugEvent2 pEvent, ref Guid riidEvent, uint dwAttrib)
		{
			if (pProcess == null)
				return VSConstants.S_OK;

			var process = (IDebugProcess3)pProcess;
			var reason = process.GetReason();
			
			if (reason != enum_DEBUG_REASON.DEBUG_REASON_USER_ATTACHED)
				return VSConstants.S_OK;

			_debugger.UnadviseDebugEventCallback(this);

			var history = ModuleRepository.Resolve<ReAttachTargets>();
			var pid = process.GetProcessId();
			var username = GetProcessUsername(pid);

			history.AddItem(new ReAttachTarget(pid, process.GetFilename(), username));
			return VSConstants.S_OK;
		}

		public int OnModeChange(DBGMODE dbgmodeNew)
		{
			var ui = ModuleRepository.Resolve<UiModule>();
			switch (dbgmodeNew)
			{
				case DBGMODE.DBGMODE_Run:
					ui.Enable(false);
					if (_debugger.AdviseDebugEventCallback(this) != VSConstants.S_OK)
						Trace.WriteLine("ReAttach: OnModeChange. AdviseDebugEventCallBack failed.");
					break;
				case DBGMODE.DBGMODE_Design:
					_debugger.UnadviseDebugEventCallback(this);
					ui.Update();
					ui.Enable(true);
					break;
			}
			return VSConstants.S_OK;
		}

		public bool TryReAttach(ReAttachTarget target)
		{
			var serviceProvider = ModuleRepository.Resolve<IServiceContainer>();
			var dte = serviceProvider.GetService(typeof(SDTE)) as DTE2;
			var debugger = dte.Debugger as Debugger3;

			Process3 process = null; // First try to use the pid.
			var processes = debugger.LocalProcesses.OfType<Process3>();
			var candidates = processes.Where(p => 
				p.Name == target.ProcessPath && 
				p.UserName == target.ProcessUser).ToList();

			if (!candidates.Any())
				return false;

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
			catch { }
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
