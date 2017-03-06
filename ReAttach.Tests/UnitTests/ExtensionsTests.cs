using EnvDTE90;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReAttach.Extensions;

namespace ReAttach.Tests.UnitTests
{
	[TestClass]
	public class ExtensionsTests
	{
		[TestMethod]
		public void GetReasonTest()
		{
			var process = new Mock<IDebugProcess3>();
			var expectedReason = (uint)enum_DEBUG_REASON.DEBUG_REASON_USER_ATTACHED;

			process.Setup(p => p.GetDebugReason(out expectedReason)).Returns(VSConstants.S_OK);
			var reason1 = process.Object.GetReason();
			Assert.AreEqual((enum_DEBUG_REASON)expectedReason, reason1, "Wrong reason returned.");

			process.Setup(p => p.GetDebugReason(out expectedReason)).Returns(VSConstants.S_FALSE);
			var reason2 = process.Object.GetReason();

			Assert.AreEqual(enum_DEBUG_REASON.DEBUG_REASON_ERROR, reason2, "Wrong reason returned, should be an error.");
		}

		[TestMethod]
		public void GetFilenameTest()
		{
			var process = new Mock<IDebugProcess2>();
			var expectedName = "cmd.exe";

			process.Setup(p => p.GetName((uint)enum_GETNAME_TYPE.GN_FILENAME, out expectedName)).Returns(VSConstants.S_OK);

			var result1 = process.Object.GetFilename();
			Assert.AreEqual(expectedName, result1, "Invalid filename returned.");

			process.Setup(p => p.GetName(It.IsAny<uint>(), out expectedName)).Returns(VSConstants.S_FALSE);
			var result2 = process.Object.GetFilename();
			Assert.AreEqual(string.Empty, result2, "Non empty filename returned even though internal function failed.");
		}

		[TestMethod]
		public void GetProcessIdTest()
		{
			var process = new Mock<IDebugProcess2>();
			const int expectedPid = 123;

			process.Setup(p => p.GetPhysicalProcessId(It.IsAny<AD_PROCESS_ID[]>())).
				Callback<AD_PROCESS_ID[]>(p => p[0].dwProcessId = expectedPid).
				Returns(VSConstants.S_OK);

			var pid1 = process.Object.GetProcessId();
			Assert.AreEqual(expectedPid, pid1, "Invalid PID returned.");

			process.Setup(p => p.GetPhysicalProcessId(It.IsAny<AD_PROCESS_ID[]>())).Returns(VSConstants.S_FALSE);
			var pid2 = process.Object.GetProcessId();
			Assert.AreEqual(0, pid2, "Valid PID returned even though function returns S_FALSE.");
		}
	
		[TestMethod]
		public void ExtensionMethodsNullTests()
		{
			Assert.IsTrue(string.IsNullOrEmpty(DebugProcessExtensions.GetFilename(null)));
			Assert.AreEqual(0, DebugProcessExtensions.GetProcessId(null));
			Assert.AreEqual(enum_DEBUG_REASON.DEBUG_REASON_ERROR, DebugProcessExtensions.GetReason(null));
		}

		[TestMethod]
		public void GetProcessUsernameTests()
		{
			var process = new Mock<Process3>();
			process.Setup(p => p.UserName).Returns("testprocess");
			Assert.AreEqual("testprocess", process.Object.GetUsername());
	
			process.Setup(p => p.UserName).Returns("testprocess [admi");
			Assert.AreEqual("testprocess", process.Object.GetUsername());

			process.Setup(p => p.UserName).Returns("testprocess [administrator]");
			Assert.AreEqual("testprocess", process.Object.GetUsername());			
		}
	}
}
