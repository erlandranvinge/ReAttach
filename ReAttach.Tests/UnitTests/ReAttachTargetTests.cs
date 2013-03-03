using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReAttach.Data;

namespace ReAttach.Tests.UnitTests
{
	[TestClass]
	public class ReAttachTargetTests
	{
		[TestMethod]
		public void EqualityTest()
		{
			var p1 = new ReAttachTarget(0, @"c:\cmd.exe", @"domain\username");
			var p2 = new ReAttachTarget(0, @"c:\CmD.eXe", @"DOMAIn\USeRNAmE");
			Assert.AreEqual(p1, p2);
			Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());

			var p3 = new ReAttachTarget(1, @"c:\cmd.exe", @"domain\username");
			var p4 = new ReAttachTarget(2, @"c:\CmD.eXe", @"DOMAIn\USeRNAmE");
			Assert.AreEqual(p3, p4);
			Assert.AreEqual(p3.GetHashCode(), p4.GetHashCode());
			Assert.IsTrue(p3.Equals(p4));
		}

		[TestMethod]
		public void InequalityTest()
		{
			var p1 = new ReAttachTarget(0, @"c:\cmd.exe", @"domain\username");
			var p2 = new ReAttachTarget(0, @"c:\calc.exe", @"domain\username");
			Assert.AreNotEqual(p1, p2);

			var p3 = new ReAttachTarget(0, @"c:\cmd.exe", @"domain\username1");
			var p4 = new ReAttachTarget(0, @"c:\cmd.exe", @"domain\username2");
			Assert.AreNotEqual(p3, p4);

			Assert.AreNotEqual(null, p1);
			Assert.AreNotEqual(null, p2);
			Assert.AreNotEqual(p3, null);
			Assert.AreNotEqual(p4, null);
			Assert.IsFalse(p3.Equals(null));
		}

		[TestMethod]
		public void InvalidPathTest()
		{
			var p1 = new ReAttachTarget(0, @"c:\cmd.exe" + Path.GetInvalidPathChars()[0], @"domain\username1");
			Assert.AreEqual(p1.ProcessName, p1.ProcessPath);
		}

		[TestMethod]
		public void IsLocalTests()
		{
			var p1 = new ReAttachTarget(0, @"c:\process1.exe", @"domain\username");
			var p2 = new ReAttachTarget(0, @"c:\process2.exe", @"domain\username", "server");

			Assert.IsTrue(p1.IsLocal);
			Assert.IsFalse(p2.IsLocal);
		}

		[TestMethod]
		public void StringFormattingTests()
		{
			var p1 = new ReAttachTarget(0, @"c:\process1.exe", @"domain\username");
			var p2 = new ReAttachTarget(0, @"c:\process2.exe", @"domain\username", "servername");
			Assert.IsTrue(p1.ToString().Contains("process1.exe"), "Formatted string doesn't contain process name.");
			Assert.IsTrue(p1.ToString().Contains(@"domain\username"), "Formatted string doesn't contain username.");

			Assert.IsTrue(p2.ToString().Contains("process2.exe"), "Formatted string doesn't contain process name.");
			Assert.IsTrue(p2.ToString().Contains(@"domain\username"), "Formatted string doesn't contain username.");
			Assert.IsTrue(p2.ToString().Contains("servername"), "Formatted string doesn't contain servername.");
			
		}
	}
}
