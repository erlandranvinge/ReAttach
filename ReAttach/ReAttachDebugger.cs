using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE80;
using EnvDTE90;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using ReAttach.Contracts;
using ReAttach.Data;
using ReAttach.Extensions;
using ReAttach.Misc;

namespace ReAttach
{
	public class ReAttachDebugger : IReAttachDebugger
	{
		private readonly IReAttachPackage _package;
		private readonly IVsDebugger _debugger;
		private readonly DTE2 _dte;
		private readonly Debugger2 _dteDebugger;
		private readonly uint _cookie;

		public ReAttachDebugger(IReAttachPackage package)
		{
			_package = package;
			_debugger = package.GetService(typeof(SVsShellDebugger)) as IVsDebugger;
			_dte = _package.GetService(typeof(SDTE)) as DTE2;
			if (_dte != null)
				_dteDebugger = _dte.Debugger as Debugger2;

			if (_package == null || _debugger == null || _dte == null || _dteDebugger == null)
			{
				_package.Reporter.ReportError(
					"Unable to get required services for ReAttachDebugger in ctor.");
				return;
			}

			if (_debugger.AdviseDebuggerEvents(this, out _cookie) != VSConstants.S_OK)
				_package.Reporter.ReportError("ReAttach: AdviserDebuggerEvents failed.");

			if (_debugger.AdviseDebugEventCallback(this) != VSConstants.S_OK)
				_package.Reporter.ReportError("AdviceDebugEventsCallback call failed in ReAttachDebugger ctor.");
		}

		public int Event(IDebugEngine2 pEngine, IDebugProcess2 process, IDebugProgram2 pProgram, 
			IDebugThread2 pThread, IDebugEvent2 debugEvent, ref Guid riidEvent, uint dwAttrib)
		{
			if (debugEvent is IDebugModuleLoadEvent2)
				return VSConstants.S_OK;

			Trace.WriteLine(TypeHelper.GetDebugEventTypeName(debugEvent)); // TODO: Remove me.

			if (process == null)
				return VSConstants.S_OK;

			var target = GetTargetFromProcess(process);
			if (target == null)
			{
				_package.Reporter.ReportWarning("Can't find target from process {0} ({1}). Event: {2}.",
					process.GetName(), process.GetProcessId(), TypeHelper.GetDebugEventTypeName(debugEvent));
				return VSConstants.S_OK;
			}
			if (debugEvent is IDebugProcessCreateEvent2)
			{
				target.IsAttached = true;
				_package.History.Items.AddFirst(target); 
				_package.Ui.Update();
				return VSConstants.S_OK;
			}

			if (debugEvent is IDebugProcessDestroyEvent2)
			{
				target.IsAttached = false;
				_package.Ui.Update();
				return VSConstants.S_OK;
			}

			return VSConstants.S_OK;
		}

		public int OnModeChange(DBGMODE mode)
		{
			if (mode == DBGMODE.DBGMODE_Design)
				_package.History.Save();
			return VSConstants.S_OK;
		}

		public ReAttachTarget GetTargetFromProcess(IDebugProcess2 debugProcess)
		{
			var process = (IDebugProcess3)debugProcess;
			var pid = process.GetProcessId();
			var target = _package.History.Items.Find(pid);
			if (target != null)
				return target;

			var user = GetProcessUsername(pid);
			var path = process.GetFilename();
			return new ReAttachTarget(pid, path, user, "");
		}

		public bool ReAttach(ReAttachTarget target)
		{
			if (target == null)
				return false;
			List<Process3> candidates;
			if (!target.IsLocal)
			{
				var transport = _dteDebugger.Transports.Item("Default");
				var processes = _dteDebugger.GetProcesses(transport, target.ServerName).OfType<Process3>();
				candidates = processes.Where(p => p.Name == target.ProcessPath).ToList();
			}
			else
			{
				var processes = _dteDebugger.LocalProcesses.OfType<Process3>();
				candidates = processes.Where(p =>
					p.Name == target.ProcessPath &&
					p.UserName == target.ProcessUser).ToList();

				if (!candidates.Any()) // Do matching on processes running in exclusive mode.
				{
					candidates = processes.Where(p => 
						p.Name == target.ProcessName &&
						string.IsNullOrEmpty(p.UserName)).ToList();
				}
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

			try
			{
				if (process != null)
				{
					process.Attach();
					return true;
				}
			}
			catch (COMException e)
			{
				// It's either this or returning this HRESULT to shell with Shell.ReportError method, shows UAC box btw.
				const int E_ELEVATION_REQUIRED = unchecked((int)0x800702E4);
				Marshal.ThrowExceptionForHR(E_ELEVATION_REQUIRED); 
				return false;
			}
			catch (Exception e)
			{
				_package.Reporter.ReportError("Unable to ReAttach to process {0} ({1}) based on target {2}. Message: {3}.", 
					process.Name, process.ProcessID, target, e.Message);
			}
			return false;
		}

		private string GetProcessUsername(int pid)
		{
			var process = _dteDebugger.LocalProcesses.OfType<Process3>().FirstOrDefault(p => p.ProcessID == pid);
			var result = process != null ? process.UserName : string.Empty;

			return result;
		}
	}
}
