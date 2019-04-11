using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReAttach.Data;
using ReAttach.Tests.Mocks;

namespace ReAttach.Tests.UnitTests
{
	[TestClass]
	public class ReAttachDebuggerTests
	{
		private readonly ReAttachMocks _mocks = new ReAttachMocks();
		
		[TestMethod]
		public async System.Threading.Tasks.Task InvalidTargetTest()
		{
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);
			Assert.IsFalse(debugger.ReAttach(null));

			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public async System.Threading.Tasks.Task ReAttachNoCandidatesTest()
		{
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);
			Assert.IsFalse(debugger.ReAttach(new ReAttachTarget(123, "not-name1", "user1")));
			Assert.IsFalse(debugger.ReAttach(new ReAttachTarget(123, "name1", "not-user1")));
			Assert.IsTrue(debugger.ReAttach(new ReAttachTarget(123, "name1", "user1")));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public async System.Threading.Tasks.Task ReAttachHighestPidTest()
		{
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);
			Assert.IsTrue(debugger.ReAttach(new ReAttachTarget(5, "name1", "user1")));
			_mocks.MockProcessList[1].Verify(p => p.Attach(), Times.Once());
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public async System.Threading.Tasks.Task ReAttachExactPidTest()
		{
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);
			Assert.IsTrue(debugger.ReAttach(new ReAttachTarget(1, "name1", "user1")));
			_mocks.MockProcessList[0].Verify(p => p.Attach(), Times.Once());
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public async System.Threading.Tasks.Task ReAttachAttachFailsTest()
		{
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);
			_mocks.MockProcessList[0].Setup(p => p.Attach()).Throws(new Exception("I'm failing. For testing purposes. :)"));
			Assert.IsFalse(debugger.ReAttach(new ReAttachTarget(1, "name1", "user1")));
			Assert.AreEqual(1, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public async System.Threading.Tasks.Task ReAttachRemoteTest()
		{
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);
			Assert.IsTrue(debugger.ReAttach(new ReAttachTarget(1, "name1", "user1", "server1")));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
			_mocks.MockDTEDebugger.Verify(d => d.GetProcesses(_mocks.MockDefaultTransport.Object, "server1"), Times.Once());
		}

		[TestMethod]
		public async System.Threading.Tasks.Task ReAttachRemoteNotFoundTest()
		{
			// TODO: Find out what happens with GetProcesses when machine is down.
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);

			Assert.IsFalse(debugger.ReAttach(new ReAttachTarget(1, "not-name1", "not-user1", "not-server1")));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);

			_mocks.MockDTEDebugger.Verify(d => d.GetProcesses(_mocks.MockDefaultTransport.Object, "not-server1"), Times.Once());
		}

		[TestMethod]
		public async System.Threading.Tasks.Task RecordAttachTest()
		{
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);
			_mocks.MockReAttachUi.Setup(ui => ui.Update()); 

			var processCreateEvent = new Mock<IDebugProcessCreateEvent2>(MockBehavior.Strict);
			var process = new Mock<IDebugProcess3>(MockBehavior.Strict);
			process.Setup(p => p.GetEngineFilter(It.IsAny<GUID_ARRAY[]>())).Returns(null);

			var process2 = process.As<IDebugProcess2>();
			process2.Setup(p => p.GetPhysicalProcessId(It.IsAny<AD_PROCESS_ID[]>())).Returns(123);
			

			var serverMock = new Mock<IDebugCoreServer3>(MockBehavior.Strict);
			var server = serverMock.As<IDebugCoreServer2>().Object;
			serverMock.Setup(s => s.QueryIsLocal()).Returns(VSConstants.S_OK);
			process2.Setup(p => p.GetServer(out server)).Returns(VSConstants.S_OK); 
			
			string processName;
			process2.Setup(p => p.GetName(It.IsAny<enum_GETNAME_TYPE>(), out processName)).Returns(VSConstants.S_OK);

			var eventGuid = Guid.Empty;
			const uint attributes = 0;
			Assert.AreEqual(VSConstants.S_OK, debugger.Event(null, process.Object, null, null, 
				processCreateEvent.As<IDebugEvent2>().Object, ref eventGuid, attributes));

			_mocks.MockReAttachHistory.Verify(h => h.Items);
			_mocks.MockReAttachUi.Verify(ui => ui.Update());
			Assert.AreEqual(1, _mocks.MockReAttachHistoryItems.Count(i => i.IsAttached), "Invalid number of processes claimed to be attached.");
		}

		[TestMethod]
		public async System.Threading.Tasks.Task RecordRemoteAttachTest()
		{
			var debugger = await ReAttachDebugger.InitAsync(_mocks.MockReAttachPackage.Object);
			_mocks.MockReAttachUi.Setup(ui => ui.Update());

			var processCreateEvent = new Mock<IDebugProcessCreateEvent2>(MockBehavior.Strict);
			var process = new Mock<IDebugProcess3>(MockBehavior.Strict);
			process.Setup(p => p.GetEngineFilter(It.IsAny<GUID_ARRAY[]>())).Returns(null);

			var process2 = process.As<IDebugProcess2>();
			process2.Setup(p => p.GetPhysicalProcessId(It.IsAny<AD_PROCESS_ID[]>())).Returns(123);

			var serverMock = new Mock<IDebugCoreServer3>(MockBehavior.Strict);
			var server = serverMock.As<IDebugCoreServer2>().Object;
			serverMock.Setup(s => s.QueryIsLocal()).Returns(VSConstants.S_FALSE);
			var serverName = "server:1234";
			serverMock.Setup(s => s.GetServerFriendlyName(out serverName)).Returns(VSConstants.S_OK);
			process2.Setup(p => p.GetServer(out server)).Returns(VSConstants.S_OK);
		
			string processName;
			process2.Setup(p => p.GetName(It.IsAny<enum_GETNAME_TYPE>(), out processName)).Returns(VSConstants.S_OK);

			var eventGuid = Guid.Empty;
			const uint attributes = 0;
			Assert.AreEqual(VSConstants.S_OK, debugger.Event(null, process.Object, null, null,
				processCreateEvent.As<IDebugEvent2>().Object, ref eventGuid, attributes));
			Assert.AreEqual("server:1234", _mocks.MockReAttachHistoryItems[0].ServerName, 
				"Invalid server name, server name not set when remote ReAttaching.");

			_mocks.MockReAttachHistory.Verify(h => h.Items);
			_mocks.MockReAttachUi.Verify(ui => ui.Update());
			Assert.AreEqual(1, _mocks.MockReAttachHistoryItems.Count(i => i.IsAttached), "Invalid number of processes claimed to be attached.");
		}
	}
}
