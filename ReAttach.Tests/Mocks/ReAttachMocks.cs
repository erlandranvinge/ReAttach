using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using EnvDTE90;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VsSDK.UnitTestLibrary;
using Moq;
using ReAttach.Contracts;
using ReAttach.Data;

namespace ReAttach.Tests.Mocks
{
	[ExcludeFromCodeCoverage]
	internal class ReAttachMocks
	{
		public List<Mock<Process3>> MockProcessList { get; private set; }
		public Mock<Processes> MockProcesses { get; private set; }
		public Mock<IVsDebugger> MockDebugger { get; private set; }
		public Mock<IMenuCommandService> MockMenuService { get; private set; }
		public Mock<DTE2> MockDTE { get; private set; }
		public Mock<Debugger2> MockDTEDebugger { get; private set; }
		public Mock<Transports> MockTransports { get; private set; }
		public Mock<Transport> MockDefaultTransport { get; private set; }

		/* ReAttach specific systems */
		public ReAttachTraceReporter MockReAttachReporter { get; private set; }
		public Mock<IReAttachUi> MockReAttachUi { get; private set; }
		public Mock<IReAttachPackage> MockReAttachPackage { get; private set; }
		public Mock<IReAttachDebugger> MockReAttachDebugger { get; private set; }
		public Mock<IReAttachHistory> MockReAttachHistory { get; private set; }
		public ReAttachTargetList MockReAttachHistoryItems { get; private set; }


		public ReAttachMocks(MockBehavior behavior = MockBehavior.Strict)
		{
			CreateMockDebugger(behavior);
			CreateMockDTE(behavior);
			CreateMockMenuCommandService(behavior);

			MockReAttachReporter = new ReAttachTraceReporter(); // Use real reporting for simplicity.
			MockReAttachUi = new Mock<IReAttachUi>(behavior);
			MockReAttachHistory = new Mock<IReAttachHistory>(behavior);
			MockReAttachHistoryItems = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);
			MockReAttachHistory.Setup(h => h.Items).Returns(MockReAttachHistoryItems);
			MockReAttachDebugger = new Mock<IReAttachDebugger>(behavior);

			// Wire up all services and modules.
			MockReAttachPackage = new Mock<IReAttachPackage>(behavior);
			MockReAttachPackage.Setup(p => p.GetService(typeof(IMenuCommandService))).Returns(MockMenuService.Object);
			MockReAttachPackage.Setup(p => p.GetService(typeof(SVsShellDebugger))).Returns(MockDebugger.Object);
			MockReAttachPackage.Setup(p => p.GetService(typeof(SDTE))).Returns(MockDTE.Object);
			MockReAttachPackage.Setup(p => p.Reporter).Returns(MockReAttachReporter);
			MockReAttachPackage.Setup(p => p.Ui).Returns(MockReAttachUi.Object);
			MockReAttachPackage.Setup(p => p.History).Returns(MockReAttachHistory.Object);
			MockReAttachPackage.Setup(p => p.Debugger).Returns(MockReAttachDebugger.Object);
		}


		private void CreateMockMenuCommandService(MockBehavior behavior)
		{
			MockMenuService = new Mock<IMenuCommandService>(MockBehavior.Strict);
			MockMenuService.Setup(m => m.AddCommand(It.IsAny<MenuCommand>()));
		}

		private void CreateMockDebugger(MockBehavior behavior)
		{
			MockDebugger = new Mock<IVsDebugger>(behavior);
			uint cookie;
			MockDebugger.Setup(d => d.AdviseDebuggerEvents(It.IsAny<IVsDebuggerEvents>(), out cookie)).Returns(VSConstants.S_OK);
			MockDebugger.Setup(d => d.AdviseDebugEventCallback(It.IsAny<object>())).Returns(VSConstants.S_OK);
		}

		private void CreateMockDTE(MockBehavior behavior)
		{
			MockDTE = new Mock<DTE2>(behavior);

			var dteCommands = new Mock<Commands>(behavior);
			var enumerable = dteCommands.As<IEnumerable<Command>>();
			enumerable.Setup(c => c.GetEnumerator()).Returns((new List<Command>()).GetEnumerator());
			MockDTE.Setup(d => d.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>()));
			MockDTE.Setup(d => d.Commands).Returns(dteCommands.Object);

			MockDTEDebugger = new Mock<Debugger2>(behavior);
			MockDTE.Setup(d => d.Debugger).Returns(MockDTEDebugger.Object);

			var mockEngines = new Mock<Engines>();
			mockEngines.Setup(e => e.GetEnumerator()).Returns((new Engine[0]).GetEnumerator());
			MockDefaultTransport = new Mock<Transport>();
			MockDefaultTransport.Setup(t => t.Engines).Returns(mockEngines.Object);
			MockTransports = new Mock<Transports>();
			MockTransports.Setup(t => t.Item("Default")).Returns(MockDefaultTransport.Object);
			MockDTEDebugger.Setup(d => d.Transports).Returns(MockTransports.Object);

			// Create sample processes to toy around with in tests.
			MockProcessList = new List<Mock<Process3>>();
			for (var i = 2; i < 10; i++)
			{
				var process = new Mock<Process3>(behavior);
				process.Setup(p => p.Name).Returns("name" + i / 2);
				process.Setup(p => p.UserName).Returns("user" + i / 2);
				process.Setup(p => p.ProcessID).Returns(i - 1);
				process.Setup(p => p.Attach());

				MockProcessList.Add(process);
			}
			MockProcesses = new Mock<Processes>();
			var processesEnumerable = MockProcesses.As<IEnumerable>();
			processesEnumerable.Setup(ie => ie.GetEnumerator()).Returns(() => MockProcessList.Select(p => p.Object).GetEnumerator());
			MockDTEDebugger.Setup(d => d.LocalProcesses).Returns(MockProcesses.Object);
			MockDTEDebugger.Setup(d => d.GetProcesses(It.IsAny<Transport>(), It.IsAny<string>())).Returns(MockProcesses.Object);
		}
	}
}