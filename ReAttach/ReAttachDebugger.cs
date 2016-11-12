using System;
using System.Collections.Generic;
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
		private readonly Dictionary<Guid, string> _engines = new Dictionary<Guid, string>(); 

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

            // TODO: Unadvise, or did I find something telling me otherwise?
			if (_debugger.AdviseDebuggerEvents(this, out _cookie) != VSConstants.S_OK)
				_package.Reporter.ReportError("ReAttach: AdviserDebuggerEvents failed.");

			if (_debugger.AdviseDebugEventCallback(this) != VSConstants.S_OK)
				_package.Reporter.ReportError("AdviceDebugEventsCallback call failed in ReAttachDebugger ctor.");

            foreach (Engine engine in _dteDebugger.Transports.Item("Default").Engines)
            {
                var engineId = Guid.Parse(engine.ID);
                if (ReAttachConstants.IgnoredDebuggingEngines.Contains(engineId))
                    continue;

                _engines.Add(engineId, engine.Name);
            }
		}

		public int Event(IDebugEngine2 engine, IDebugProcess2 process, IDebugProgram2 program, 
			IDebugThread2 thread, IDebugEvent2 debugEvent, ref Guid riidEvent, uint attributes)
		{
             _package.Reporter.ReportTrace(TypeHelper.GetDebugEventTypeName(debugEvent));

             if (!(debugEvent is IDebugLoadCompleteEvent2) &&
				 !(debugEvent is IDebugProcessDestroyEvent2))
				return VSConstants.S_OK;

			var target = GetTargetFromProcess(process);
			if (target == null)
			{
				_package.Reporter.ReportWarning("Can't find target from process {0} ({1}). Event: {2}.",
					process.GetName(), process.GetProcessId(), TypeHelper.GetDebugEventTypeName(debugEvent));
				return VSConstants.S_OK;
			}

            if (debugEvent is IDebugLoadCompleteEvent2)
			{
                var programName = "";
                if (program != null)
                {
                    program.GetName(out programName);
                }
                if (target.Engines.Any(engineId => ReAttachConstants.IgnoredDebuggingEngines.Contains(engineId)))
                    return VSConstants.S_OK;
                /*
                var engines = target.Engines.Where(e => _engines.ContainsKey(e)).Select(e => _engines[e]).ToArray();
                var mode = new DBGMODE[1];
                _debugger.GetMode(mode);
                if (mode[0] == DBGMODE.DBGMODE_Design)
                    return VSConstants.S_OK;
                */
				target.IsAttached = true;
				_package.History.Items.AddFirst(target); 
				_package.Ui.Update();
			}
			else
			{
				target.IsAttached = false;
				_package.Ui.Update();
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

			var serverName = "";
			IDebugCoreServer2 server;
			if (debugProcess.GetServer(out server) == VSConstants.S_OK)
			{
				var server3 = server as IDebugCoreServer3;
				var tmp = "";
				if (server3 != null && server3.QueryIsLocal() == VSConstants.S_FALSE && 
					server3.GetServerFriendlyName(out tmp) == VSConstants.S_OK)
				{
					serverName = tmp;
				}
			}

			var user = GetProcessUsername(pid);
			var path = process.GetFilename();
			target = new ReAttachTarget(pid, path, user, serverName);

			var rawEngines = new GUID_ARRAY[1];

			if (process.GetEngineFilter(rawEngines) == VSConstants.S_OK && rawEngines[0].dwCount > 0)
			{
				var pointer = rawEngines[0].Members;
				var engineCount = rawEngines[0].dwCount;
				for (var i = 0; i < engineCount; i++)
				{
					var engineId = (Guid)Marshal.PtrToStructure(pointer + (i * 16), typeof(Guid));
					target.Engines.Add(engineId);
				}
			}
			return target;
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

                var tmp = processes.Select(p => new { Name = p.Name, UserName = p.UserName }).ToArray();
                Console.WriteLine(tmp);

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

			if (process == null)
				return false;

			try
			{
				if (target.Engines != null && target.Engines.Any())
				{
					var engines = target.Engines.Where(e => _engines.ContainsKey(e)).Select(e => _engines[e]).ToArray();
					process.Attach2(engines);
				}
				else
				{
					process.Attach();
				}
				return true;
			}
			catch (COMException e)
			{
                _package.Reporter.ReportError("Unable to ReAttach to process {0} ({1}) based on target {2}. Message: {3}.",
                    process.Name, process.ProcessID, target, e.Message);

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
