using System.Diagnostics.CodeAnalysis;
using EnvDTE;
using EnvDTE80;
using EnvDTE90;
using Microsoft.VisualStudio.Debugger.Interop;

namespace ReAttach.Misc
{
	[ExcludeFromCodeCoverage]
	internal static class TypeHelper
	{
		public static string GetDebugEventTypeName(object o)
		{
			var result = string.Empty;

			if (o is IDebugActivateDocumentEvent2) result += " IDebugActivateDocumentEvent2";
			if (o is IDebugBeforeSymbolSearchEvent2) result += " IDebugBeforeSymbolSearchEvent2";
			if (o is IDebugBoundBreakpoint2) result += " IDebugBoundBreakpoint2";
			if (o is IDebugBreakEvent2) result += " IDebugBreakEvent2";
			if (o is IDebugBreakpointBoundEvent2) result += " IDebugBreakpointBoundEvent2";
			if (o is IDebugBreakpointChecksumRequest2) result += " IDebugBreakpointChecksumRequest2";
			if (o is IDebugBreakpointErrorEvent2) result += " IDebugBreakpointErrorEvent2";
			if (o is IDebugBreakpointEvent2) result += " IDebugBreakpointEvent2";
			if (o is IDebugBreakpointRequest2) result += " IDebugBreakpointRequest2";
			if (o is IDebugBreakpointRequest3) result += " IDebugBreakpointRequest3";
			if (o is IDebugBreakpointResolution2) result += " IDebugBreakpointResolution2";
			if (o is IDebugBreakpointUnboundEvent2) result += " IDebugBreakpointUnboundEvent2";
			if (o is IDebugCanStopEvent2) result += " IDebugCanStopEvent2";
			if (o is IDebugCodeContext2) result += " IDebugCodeContext2";
			if (o is IDebugCodeContext3) result += " IDebugCodeContext3";
			if (o is IDebugCoreServer2) result += " IDebugCoreServer2";
			if (o is IDebugCoreServer3) result += " IDebugCoreServer3";
			if (o is IDebugCustomViewer) result += " IDebugCustomViewer";
			if (o is IDebugDefaultPort2) result += " IDebugDefaultPort2";
			if (o is IDebugDisassemblyStream2) result += " IDebugDisassemblyStream2";
			if (o is IDebugDocument2) result += " IDebugDocument2";
			if (o is IDebugDocumentChecksum2) result += " IDebugDocumentChecksum2";
			if (o is IDebugDocumentContext2) result += " IDebugDocumentContext2";
			if (o is IDebugDocumentPosition2) result += " IDebugDocumentPosition2";
			if (o is IDebugDocumentPositionOffset2) result += " IDebugDocumentPositionOffset2";
			if (o is IDebugDocumentText2) result += " IDebugDocumentText2";
			if (o is IDebugDocumentTextEvents2) result += " IDebugDocumentTextEvents2";
			if (o is IDebugEngine2) result += " IDebugEngine2";
			if (o is IDebugEngine3) result += " IDebugEngine3";
			if (o is IDebugEngineCreateEvent2) result += " IDebugEngineCreateEvent2";
			if (o is IDebugEngineLaunch2) result += " IDebugEngineLaunch2";
			if (o is IDebugEngineProgram2) result += " IDebugEngineProgram2";
			if (o is IDebugEntryPointEvent2) result += " IDebugEntryPointEvent2";
			if (o is IDebugErrorBreakpoint2) result += " IDebugErrorBreakpoint2";
			if (o is IDebugErrorBreakpointResolution2) result += " IDebugErrorBreakpointResolution2";
			if (o is IDebugErrorEvent2) result += " IDebugErrorEvent2";
			if (o is IDebugEvent2) result += " IDebugEvent2";
			if (o is IDebugEventCallback2) result += " IDebugEventCallback2";
			if (o is IDebugExceptionEvent2) result += " IDebugExceptionEvent2";
			if (o is IDebugExpression2) result += " IDebugExpression2";
			if (o is IDebugExpressionContext2) result += " IDebugExpressionContext2";
			if (o is IDebugExpressionEvaluationCompleteEvent2) result += " IDebugExpressionEvaluationCompleteEvent2";
			if (o is IDebugFirewallConfigurationCallback2) result += " IDebugFirewallConfigurationCallback2";
			if (o is IDebugFunctionPosition2) result += " IDebugFunctionPosition2";
			if (o is IDebugInterceptExceptionCompleteEvent2) result += " IDebugInterceptExceptionCompleteEvent2";
			if (o is IDebugLoadCompleteEvent2) result += " IDebugLoadCompleteEvent2";
			if (o is IDebugMemoryBytes2) result += " IDebugMemoryBytes2";
			if (o is IDebugMemoryContext2) result += " IDebugMemoryContext2";
			if (o is IDebugMessageEvent2) result += " IDebugMessageEvent2";
			if (o is IDebugModule2) result += " IDebugModule2";
			if (o is IDebugModule3) result += " IDebugModule3";
			if (o is IDebugModuleLoadEvent2) result += " IDebugModuleLoadEvent2";
			if (o is IDebugNoSymbolsEvent2) result += " IDebugNoSymbolsEvent2";
			if (o is IDebugOutputStringEvent2) result += " IDebugOutputStringEvent2";
			if (o is IDebugPendingBreakpoint2) result += " IDebugPendingBreakpoint2";
			if (o is IDebugPort2) result += " IDebugPort2";
			if (o is IDebugPortEvents2) result += " IDebugPortEvents2";
			if (o is IDebugPortEx2) result += " IDebugPortEx2";
			if (o is IDebugPortNotify2) result += " IDebugPortNotify2";
			if (o is IDebugPortPicker) result += " IDebugPortPicker";
			if (o is IDebugPortRequest2) result += " IDebugPortRequest2";
			if (o is IDebugPortSupplier2) result += " IDebugPortSupplier2";
			if (o is IDebugPortSupplier3) result += " IDebugPortSupplier3";
			if (o is IDebugPortSupplierEx2) result += " IDebugPortSupplierEx2";
			if (o is IDebugPortSupplierLocale2) result += " IDebugPortSupplierLocale2";
			if (o is IDebugPortSupplierDescription2) result += " IDebugPortSupplierDescription2";
			if (o is IDebugProcess2) result += " IDebugProcess2";
			if (o is IDebugProcess3) result += " IDebugProcess3";
			if (o is IDebugProcessCreateEvent2) result += " IDebugProcessCreateEvent2";
			if (o is IDebugProcessDestroyEvent2) result += " IDebugProcessDestroyEvent2";
			if (o is IDebugProcessEx2) result += " IDebugProcessEx2";
			if (o is IDebugProcessQueryProperties) result += " IDebugProcessQueryProperties";
			if (o is IDebugProgram2) result += " IDebugProgram2";
			if (o is IDebugProgram3) result += " IDebugProgram3";
			if (o is IDebugProgramCreateEvent2) result += " IDebugProgramCreateEvent2";
			if (o is IDebugProgramDestroyEvent2) result += " IDebugProgramDestroyEvent2";
			if (o is IDebugProgramDestroyEventFlags2) result += " IDebugProgramDestroyEventFlags2";
			if (o is IDebugProgramEngines2) result += " IDebugProgramEngines2";
			if (o is IDebugProgramEx2) result += " IDebugProgramEx2";
			if (o is IDebugProgramHost2) result += " IDebugProgramHost2";
			if (o is IDebugProgramNameChangedEvent2) result += " IDebugProgramNameChangedEvent2";
			if (o is IDebugProgramNode2) result += " IDebugProgramNode2";
			if (o is IDebugProgramNodeAttach2) result += " IDebugProgramNodeAttach2";
			if (o is IDebugProgramProvider2) result += " IDebugProgramProvider2";
			if (o is IDebugProgramPublisher2) result += " IDebugProgramPublisher2";
			if (o is IDebugProperty2) result += " IDebugProperty2";
			if (o is IDebugProperty3) result += " IDebugProperty3";
			if (o is IDebugPropertyCreateEvent2) result += " IDebugPropertyCreateEvent2";
			if (o is IDebugPropertyDestroyEvent2) result += " IDebugPropertyDestroyEvent2";
			if (o is IDebugProviderProgramNode2) result += " IDebugProviderProgramNode2";
			if (o is IDebugQueryEngine2) result += " IDebugQueryEngine2";
			if (o is IDebugReference2) result += " IDebugReference2";
			if (o is IDebugReturnValueEvent2) result += " IDebugReturnValueEvent2";
			if (o is IDebugSettingsCallback2) result += " IDebugSettingsCallback2";
			if (o is IDebugSourceServerModule) result += " IDebugSourceServerModule";
			if (o is IDebugStackFrame2) result += " IDebugStackFrame2";
			if (o is IDebugStackFrame3) result += " IDebugStackFrame3";
			if (o is IDebugStepCompleteEvent2) result += " IDebugStepCompleteEvent2";
			if (o is IDebugStopCompleteEvent2) result += " IDebugStopCompleteEvent2";
			if (o is IDebugSymbolSearchEvent2) result += " IDebugSymbolSearchEvent2";
			if (o is IDebugThread2) result += " IDebugThread2";
			if (o is IDebugThreadCreateEvent2) result += " IDebugThreadCreateEvent2";
			if (o is IDebugThreadDestroyEvent2) result += " IDebugThreadDestroyEvent2";
			if (o is IDebugThreadNameChangedEvent2) result += " IDebugThreadNameChangedEvent2";
			if (o is IDebugWindowsComputerPort2) result += " IDebugWindowsComputerPort2";
			return result;
		}

		public static string GetProcessTypeName(object o)
		{
			if (o is Process3) return "Process3";
			if (o is Process2) return "Process2";
			if (o is Process) return "Process";
			return "Unknown process type. :/";
		}
	}
}