using System;
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
		public void ReAttachInvalidTargetTest()
		{
			var debugger = new ReAttachDebugger(_mocks.MockReAttachPackage.Object);
			Assert.IsFalse(debugger.ReAttach(null));

			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public void ReAttachNoCandidatesTest()
		{
			var debugger = new ReAttachDebugger(_mocks.MockReAttachPackage.Object);
			Assert.IsFalse(debugger.ReAttach(new ReAttachTarget(123, "not-name1", "user1")));
			Assert.IsFalse(debugger.ReAttach(new ReAttachTarget(123, "name1", "not-user1")));
			Assert.IsTrue(debugger.ReAttach(new ReAttachTarget(123, "name1", "user1")));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public void ReAttachHighestPidTest()
		{
			var debugger = new ReAttachDebugger(_mocks.MockReAttachPackage.Object);
			Assert.IsTrue(debugger.ReAttach(new ReAttachTarget(5, "name1", "user1")));
			_mocks.MockProcessList[1].Verify(p => p.Attach(), Times.Once());
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public void ReAttachExactPidTest()
		{
			var debugger = new ReAttachDebugger(_mocks.MockReAttachPackage.Object);
			Assert.IsTrue(debugger.ReAttach(new ReAttachTarget(1, "name1", "user1")));
			_mocks.MockProcessList[0].Verify(p => p.Attach(), Times.Once());
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public void ReAttachAttachFailsTest()
		{
			var debugger = new ReAttachDebugger(_mocks.MockReAttachPackage.Object);
			_mocks.MockProcessList[0].Setup(p => p.Attach()).Throws(new Exception("I'm failing. For testing purposes. :)"));
			Assert.IsFalse(debugger.ReAttach(new ReAttachTarget(1, "name1", "user1")));
			Assert.AreEqual(1, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
		}

		[TestMethod]
		public void ReAttachRemoteTest()
		{
			var debugger = new ReAttachDebugger(_mocks.MockReAttachPackage.Object);
			Assert.IsTrue(debugger.ReAttach(new ReAttachTarget(1, "name1", "user1", "server1")));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);
			_mocks.MockDTEDebugger.Verify(d => d.GetProcesses(_mocks.MockDefaultTransport.Object, "server1"), Times.Once());
		}

		[TestMethod]
		public void ReAttachRemoteNotFoundTest()
		{
			// TODO: Find out what happens with GetProcesses when machine is down.
			var debugger = new ReAttachDebugger(_mocks.MockReAttachPackage.Object);

			Assert.IsFalse(debugger.ReAttach(new ReAttachTarget(1, "not-name1", "not-user1", "not-server1")));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount);
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount);

			_mocks.MockDTEDebugger.Verify(d => d.GetProcesses(_mocks.MockDefaultTransport.Object, "not-server1"), Times.Once());
		}

		[TestMethod]
		public void ReAttachEventsTest()
		{
			var debugger = new ReAttachDebugger(_mocks.MockReAttachPackage.Object);
			var eventGuid = Guid.Empty;
			const uint attributes = 0;
			debugger.Event(null, null, null, null, null, ref eventGuid, attributes);
		}
	}
}
