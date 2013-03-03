using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using ReAttach.Tests.Mocks;

namespace ReAttach.Tests.UnitTests
{
	[TestClass]
	public class ReAttachPackageTests
	{
		private readonly ReAttachMocks _mocks = new ReAttachMocks();

		[TestMethod]
		public void InitializationTest()
		{
			var mockServiceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();
			mockServiceProvider.AddService(typeof(SVsShellDebugger), _mocks.MockDebugger.Object, false);
			mockServiceProvider.AddService(typeof(SDTE), _mocks.MockDTE.Object, false);

			var package = new ReAttachPackage() as IVsPackage;
			Assert.IsNotNull(package, "The object does not implement IVsPackage");

			Assert.AreEqual(0, package.SetSite(mockServiceProvider), "SetSite did not return S_OK");
			Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");

			var reAttachPackage = (ReAttachPackage) package;
			Assert.IsNotNull(reAttachPackage.Reporter);
			Assert.IsNotNull(reAttachPackage.Ui);
			Assert.IsNotNull(reAttachPackage.History);
			Assert.IsNotNull(reAttachPackage.Debugger);

			// Check for warnings/error. Note that one warning for empty registry on first load is expected.
			Assert.AreEqual(0, reAttachPackage.Reporter.ErrorCount, "ReAttach encountered errors during initialization.");
			Assert.AreEqual(1, reAttachPackage.Reporter.WarningCount, "ReAttach encountered warnings during initialization.");
		}
	}
}
