using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ReAttach.Data;
using ReAttach.Tests.Mocks;
using VsShell = Microsoft.VisualStudio.Shell;

namespace ReAttach.Tests.UnitTests
{
	[TestClass]
	public class ReAttachUiTests
	{
		private readonly ReAttachMocks _mocks = new ReAttachMocks();

		[TestMethod]
		public async Task UiInitializationTest()
		{
			var ui = await ReAttachUi.InitAsync(_mocks.MockReAttachPackage.Object);
			_mocks.MockMenuService.Verify(m => m.AddCommand(It.IsAny<MenuCommand>()), 
				Times.Exactly(ReAttachConstants.ReAttachHistorySize));
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount, "Unexpected number of ReAttach warnings.");
		}
		
		[TestMethod]
		public async Task ReAttachCommandClickedEmptyHistory()
		{
			var ui = await ReAttachUi.InitAsync(_mocks.MockReAttachPackage.Object);
			await ui.ReAttachCommandClickedAsync(new VsShell.OleMenuCommand((sender, args) => {}, 
				new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId)), null);

			_mocks.MockReAttachDebugger.Verify(d => d.ReAttach(It.IsAny<ReAttachTarget>()), Times.Never());
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount, "Unexpected number of ReAttach warnings.");
		}

		[TestMethod]
		public async Task WillDoReAttachIfHistoryItemsArePresent()
		{
			var ui = await ReAttachUi.InitAsync(_mocks.MockReAttachPackage.Object);
			_mocks.MockReAttachDebugger.Setup(d => d.ReAttach(It.IsAny<ReAttachTarget>())).Returns(true);

			for (var i = 1; i < 5; i++)
				_mocks.MockReAttachHistoryItems.AddLast(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(1, _mocks.MockReAttachHistoryItems[0].ProcessId, "Wrong target on top of ReAttach list.");

			var id = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId + 3);
			var command = new VsShell.OleMenuCommand((sender, args) => { }, id);
			await ui.ReAttachCommandClickedAsync(command, null);

			_mocks.MockReAttachDebugger.Verify(d => d.ReAttach(_mocks.MockReAttachHistoryItems[3]), Times.Once());

			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount, "Unexpected number of ReAttach warnings.");
		}

		[TestMethod]
		public async Task NoDialogShownWhenTargetIsAlreadyRunning()
		{
			var ui = await ReAttachUi.InitAsync(_mocks.MockReAttachPackage.Object);
			_mocks.MockReAttachDebugger.Setup(d => d.ReAttach(It.IsAny<ReAttachTarget>())).Returns(true);
			_mocks.MockReAttachHistoryItems.AddFirst(new ReAttachTarget(123, "path", "user"));

			var id = new CommandID(ReAttachConstants.ReAttachPackageCmdSet, ReAttachConstants.ReAttachCommandId);
			var command = new VsShell.OleMenuCommand((sender, args) => { }, id);
			await ui.ReAttachCommandClickedAsync(command, null);

			_mocks.MockReAttachDebugger.Verify(d => d.ReAttach(It.IsAny<ReAttachTarget>()), Times.Once());
			Assert.AreEqual(0, _mocks.MockReAttachReporter.ErrorCount, "Unexpected number of ReAttach errors.");
			Assert.AreEqual(0, _mocks.MockReAttachReporter.WarningCount, "Unexpected number of ReAttach warnings.");
		}

		[TestMethod]
		public async Task CommandsShouldBeVisibleIfTheirInHistoryAndNotAttached()
		{
			var ui = await ReAttachUi.InitAsync(_mocks.MockReAttachPackage.Object);
			_mocks.MockReAttachDebugger.Setup(d => d.ReAttach(It.IsAny<ReAttachTarget>())).Returns(true);
			for (var i = 1; i <= 3; i++)
				_mocks.MockReAttachHistoryItems.AddFirst(new ReAttachTarget(123, "name" + i, "user" + i));

			ui.Update();
			Assert.AreEqual(3, ui.Commands.Count(c => c.Visible), "Incorrect number of commands visible.");

			_mocks.MockReAttachHistoryItems[2].IsAttached = true;
			ui.Update();
			Assert.AreEqual(2, ui.Commands.Count(c => c.Visible), "Incorrect number of commands visible.");
		}
	}
}
