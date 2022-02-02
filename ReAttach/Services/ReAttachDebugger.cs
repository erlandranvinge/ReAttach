using EnvDTE80;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using EnvDTE90;
using ReAttach.Models;
using ReAttach.Extensions;
using System.Runtime.InteropServices;
using ReAttach.Stores;

namespace ReAttach.Services
{
    public enum ReAttachResult
    {
        Success,
        NotStarted,
        ElevationRequired,
        Failed,
    }

    public class ReAttachDebugger: IVsDebuggerEvents, IDebugEventCallback2
    {
        private ReAttachHistory _history;
        private ReAttachUi _ui;
        private Debugger2 _dteDebugger;
        private uint _cookie;
        private Dictionary<Guid, string> _engines;
        private bool _recording;

        public async Task InitializeAsync(ReAttachPackage package, ReAttachHistory history, CancellationToken cancellationToken)
        {
            _history = history;
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);


            _ui = (await package.GetServiceAsync(typeof(ReAttachUi))) as ReAttachUi;
            if (_ui == null)
            {
                ReAttachUtils.ShowStartupError("Unable to obtain reference to UI.");
                return;
            }

            var debugger = (await package.GetServiceAsync(typeof(IVsDebugger))) as IVsDebugger;
            if (debugger == null) 
            {
                ReAttachUtils.ShowStartupError("Unable to obtain reference to debugger.");
                return;
            }

            if (debugger.AdviseDebugEventCallback(this) != VSConstants.S_OK)
            {
                ReAttachUtils.ShowStartupError("Unable to subscribe on debug events.");
                return;
            }

            if (debugger.AdviseDebuggerEvents(this, out _cookie) != VSConstants.S_OK)
            {
                ReAttachUtils.ShowStartupError("Unable to subscribe on debugger mode changes.");
                return;
            }


            var dte = await package.GetServiceAsync(typeof(EnvDTE.DTE)) as DTE2;
            if (dte == null)
            {
                ReAttachUtils.ShowStartupError("Unable to get obtain reference to automation object model (DTE2).");
                return;
            }

            _dteDebugger = dte.Debugger as Debugger2;
            if (_dteDebugger == null)
            {
                ReAttachUtils.ShowStartupError("Unable to get reference to debugger from automation object.");
                return;
            }

            _engines = GetTransportEngines();
        }

        public int OnModeChange(DBGMODE mode)
        {
            if (_history == null)
                return VSConstants.S_OK;

            switch (mode)
            {
                case DBGMODE.DBGMODE_Run:
                    _recording = true;
                    break;
                case DBGMODE.DBGMODE_Design:
                    _recording = false;
                    _history.Save();
                    break;
            }
            return VSConstants.S_OK;
        }

        public int Event(IDebugEngine2 engine, IDebugProcess2 process, IDebugProgram2 program, IDebugThread2 thread, IDebugEvent2 evt, ref Guid riidEvent, uint dwAttrib)
        {
            if (!_recording) return VSConstants.S_OK;

            if (!(evt is IDebugProcessCreateEvent2) &&
                !(evt is IDebugProcessDestroyEvent2) &&
                !(evt is IDebugEntryPointEvent2))
                return VSConstants.S_OK;

            var target = GetTargetFromProcess(process);
            if (target == null)
            {

                //_package.Reporter.ReportWarning("Can't find target from process {0} ({1}). Event: {2}.",
                //  process.GetName(), process.GetProcessId(), TypeHelper.GetDebugEventTypeName(debugEvent));
                return VSConstants.S_OK;
            }

            if (evt is IDebugProcessCreateEvent2 || evt is IDebugEntryPointEvent2)
            {
                target.IsAttached = true;
                _history.AddFirst(target);
                _ui.Update();
            }
            else
            {
                target.IsAttached = false;
                _ui.Update();
            }

            return VSConstants.S_OK;
        }

        public ReAttachResult ReAttach(ReAttachTarget target)
        {
            if (target == null)
                return ReAttachResult.Failed;

            List<Process3> candidates;
            if (!target.IsLocal)
            {
                var transport = _dteDebugger.Transports.Item("Default");
                var processes = _dteDebugger.GetProcesses(transport, target.ServerName).OfType<Process3>();
                candidates = processes.Where(p => p.IsMatchingRemoteProcess(target)).ToList();
            }
            else
            {
                var processes = _dteDebugger.LocalProcesses.OfType<Process3>();
                candidates = processes.Where(p => p.IsMatchingLocalProcess(target)).ToList();
                if (!candidates.Any()) // Do matching on processes running in exclusive mode.
                {
                    candidates = processes.Where(p => p.IsMatchingExclusively(target)).ToList();
                }
            }

            if (!candidates.Any())
                return ReAttachResult.NotStarted;

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
                return ReAttachResult.NotStarted;

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
                return ReAttachResult.Success;
            }
            catch (COMException)
            {
                return ReAttachResult.ElevationRequired;
            }
            catch (Exception e)
            {
                ReAttachUtils.ShowError($"Unable to ReAttach to process {process.Name} ({process.ProcessID}) based on target {target}", e.Message);
            }
            return ReAttachResult.Failed;
        }


        private Dictionary<Guid, string> GetTransportEngines()
        {
            var engines = new Dictionary<Guid, string>();
            try
            {
                var ignoredEngines = new HashSet<Guid>(new[] { new Guid("2c18241e-069a-43b2-bd81-89c186af994b") });

                foreach (Transport transport in _dteDebugger.Transports)
                {
                    foreach (Engine engine in transport.Engines)
                    {
                        var engineId = Guid.Parse(engine.ID);
                        if (ignoredEngines.Contains(engineId) || engines.ContainsKey(engineId))
                            continue;

                        engines.Add(engineId, engine.Name);
                    }
                }
            }
            catch (Exception e)
            {
                ReAttachUtils.ShowStartupError("Unable to get debugging engines for transports: " + e.Message);
            }
            return engines;
        }

        private ReAttachTarget GetTargetFromProcess(IDebugProcess2 debugProcess)
        {
            var process = (IDebugProcess3)debugProcess;
            var pid = process.GetProcessId();
            var target = _history.FindByPid(pid);
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

        private string GetProcessUsername(int pid)
        {
            var process = _dteDebugger.LocalProcesses.OfType<Process3>().FirstOrDefault(p => p.ProcessID == pid);
            var result = process != null ? process.UserName : string.Empty;
            return result;
        }
    }
}
