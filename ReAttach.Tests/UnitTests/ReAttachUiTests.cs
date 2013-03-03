using System.ComponentModel.Design;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.UnitTestLibrary;
using Moq;
using ReAttach.Contracts;
using ReAttach.Data;
using ReAttach.Tests.Mocks;

namespace ReAttach.Tests.UnitTests
{
	[TestClass]
	public class ReAttachUiTests
	{
		private readonly ReAttachMocks _mocks = new ReAttachMocks();

		/*
		[TestInitialize]
		public void Initialize()
		{
			_traceReporting = new ReAttachTraceReporter();
			
			_history = new Mock<IReAttachHistory>(MockBehavior.Strict);
			var items = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);
			for (var i = 1; i <= 3; i++)
				items.AddLast(new ReAttachTarget(i, "name" + i, "user" + i));
			_history.Setup(p => p.Items).Returns(items);

			_debugger = new Mock<IReAttachDebugger>(MockBehavior.Strict);

			_menuService = new Mock<IMenuCommandService>(MockBehavior.Strict);
			_menuService.Setup(m => m.AddCommand(It.IsAny<MenuCommand>()));

			_dte = new Mock<DTE2>(MockBehavior.Strict);
			_dte.Setup(d => d.ExecuteCommand(It.IsAny<string>(), It.IsAny<string>()));


			/* Set up actual fake package with all its services 
			_package = new Mock<IReAttachPackage>(MockBehavior.Strict);
			_package.Setup(p => p.Reporter).Returns(_traceReporting);
			_package.Setup(p => p.History).Returns(_history.Object);
			_package.Setup(p => p.Debugger).Returns(_debugger.Object);

			_package.Setup(p => p.GetService(typeof (IMenuCommandService))).Returns(_menuService.Object);
			_package.Setup(p => p.GetService(typeof(SDTE))).Returns(_dte.Object);	
		}
		*/

		[TestMethod]
		public void UiInitializationTest()
		{
			var ui = new ReAttachUi(_mocks.MockReAttachPackage.Object);
			_mocks.MockMenuService.Verify(m => m.AddCommand(It.IsAny<MenuCommand>()), 
				Times.Exactly(ReAttachConstants.ReAttachHistorySize));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount, "Unexpected number of ReAttach warnings.");
		}
		
		[TestMethod]
		public void ReAttachCommandClickedEmptyHistory()
		{
			var ui = new ReAttachUi(_mocks.MockReAttachPackage.Object);
			ui.ReAttachCommandClicked(null, null);
			_mocks.MockDTE.Verify(d => d.ExecuteCommand("Debug.AttachToProcess", string.Empty));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount, "Unexpected number of ReAttach warnings.");
		}

		[TestMethod]
		public void WillDoReAttachIfHistoryItemsArePresent()
		{
			var ui = new ReAttachUi(_mocks.MockReAttachPackage.Object);
			_mocks.MockReAttachDebugger.Setup(d => d.ReAttach(It.IsAny<ReAttachTarget>())).Returns(true);

			for (var i = 1; i < 5; i++)
				_mocks.MockReAttachHistoryItems.AddLast(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(1, _mocks.MockReAttachHistoryItems[0].ProcessId, "Wrong target on top of ReAttach list.");

			var id = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId + 3);
			var command = new OleMenuCommand((sender, args) => { }, id);
			ui.ReAttachCommandClicked(command, null);

			_mocks.MockReAttachDebugger.Verify(d => d.ReAttach(_mocks.MockReAttachHistoryItems[3]), Times.Once());

			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount, "Unexpected number of ReAttach warnings.");
		}

		[TestMethod]
		public void NoDialogShownWhenTargetIsAlreadyRunning()
		{
			var ui = new ReAttachUi(_mocks.MockReAttachPackage.Object);
			_mocks.MockReAttachDebugger.Setup(d => d.ReAttach(It.IsAny<ReAttachTarget>())).Returns(true);
			_mocks.MockReAttachHistoryItems.AddFirst(new ReAttachTarget(123, "path", "user"));

			var id = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId);
			var command = new OleMenuCommand((sender, args) => { }, id);
			ui.ReAttachCommandClicked(command, null);

			_mocks.MockReAttachDebugger.Verify(d => d.ReAttach(It.IsAny<ReAttachTarget>()), Times.Once());
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount, "Unexpected number of ReAttach warnings.");
		}
	/*
 * 
		[TestMethod] 
		public void MissingServicesTests()
		{
			_package.Setup(p => p.GetService(typeof(IMenuCommandService))).Returns(null);
			_package.Setup(p => p.GetService(typeof(SDTE))).Returns(null);	

			var ui = new ReAttachUi(_package.Object);
			ui.ReAttachCommandClicked(null, null);
			Assert.AreEqual(1, _traceReporting.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(1, _traceReporting.WarningCount, "Unexpected number of ReAttach warnings.");
		}*/
	}
}
