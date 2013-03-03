
using System;
using System.ComponentModel.Design;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReAttach.Tests.Misc;

namespace ReAttach.Tests.IntegrationTests
{
	[TestClass]
	public class Testing
	{
		private delegate void ThreadInvoker();
		public TestContext TestContext { get; set; }

		[TestMethod]
		[HostType("VS IDE")]
		public void PackageLoadTest()
		{
			UIThreadInvoker.Invoke((ThreadInvoker)(() =>
			{
				var shell = VsIdeTestHostContext.ServiceProvider.GetService(typeof (SVsShell)) as IVsShell;
				Assert.IsNotNull(shell);

				IVsPackage package;
				var packageGuid = new Guid(ReAttachConstants.ReAttachPackageGuidString);
				Assert.IsTrue(0 == shell.LoadPackage(ref packageGuid, out package));
				Assert.IsNotNull(package, "Package failed to load");
			}));
		}

		[TestMethod]
		[HostType("VS IDE")]
		public void WaitingForProcessDialogTest()
		{
			var command = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId);
			TestUtils.ExecuteCommand(command);
		}
	}
}
