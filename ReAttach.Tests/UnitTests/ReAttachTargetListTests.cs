using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReAttach.Data;

namespace ReAttach.Tests.UnitTests
{
	[TestClass]
	public class ReAttachTargetListTests
	{
		[TestMethod]
		public void MaxItemsTest()
		{
			var list = new ReAttachTargetList(5);
			for (var i = 1; i <= 10; i++)
				list.AddFirst(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(5, list.Count);
			Assert.AreEqual(10, list[0].ProcessId);
			Assert.AreEqual(6, list[4].ProcessId);
		}

		[TestMethod]
		public void AddFirstTest()
		{
			var list = new ReAttachTargetList(5);
			for (var i = 1; i <= 3; i++)
				list.AddFirst(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(3, list[0].ProcessId);
			Assert.AreEqual(2, list[1].ProcessId);

			for (var i = 4; i <= 5; i++)
				list.AddFirst(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(5, list[0].ProcessId);
			Assert.AreEqual(4, list[1].ProcessId);
		}

		[TestMethod]
		public void AddLastTest()
		{
			var list = new ReAttachTargetList(5);
			for (var i = 1; i <= 3; i++)
				list.AddLast(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(3, list[2].ProcessId);
			Assert.AreEqual(2, list[1].ProcessId);

			for (var i = 4; i <= 5; i++)
				list.AddLast(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(5, list[4].ProcessId);
			Assert.AreEqual(4, list[3].ProcessId);
		}
		
		[TestMethod]
		public void ContainsTest()
		{
			var list = new ReAttachTargetList(5);
			var indices = new[] {1, 2, 3, 4, 5};

			for (var i = 1; i <= 5; i++)
				list.AddLast(new ReAttachTarget(indices[i-1], "path" + i, "user" + i));
			foreach (var index in indices)
			{
				Assert.AreEqual(index, list[index-1].ProcessId);
			}
			Assert.IsNull(list[123]);
		}

		[TestMethod]
		public void AddExistingElementFirstTest()
		{
			var list = new ReAttachTargetList(5);
			for (var i = 1; i <= 4; i++)
				list.AddLast(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(3, list[2].ProcessId);
			Assert.AreEqual(4, list.Count);

			list.AddFirst(new ReAttachTarget(3, "path3", "user3"));
			Assert.AreNotEqual(3, list[2].ProcessId);
			Assert.AreEqual(3, list[0].ProcessId);
			Assert.AreEqual(4, list.Count);
		
		}

		[TestMethod]
		public void AddExistingElementLastTest()
		{
			var list = new ReAttachTargetList(5);
			for (var i = 1; i <= 4; i++)
				list.AddLast(new ReAttachTarget(i, "path" + i, "user" + i));
			Assert.AreEqual(3, list[2].ProcessId);
			Assert.AreEqual(4, list.Count);

			list.AddLast(new ReAttachTarget(3, "path3", "user3"));
			Assert.AreNotEqual(3, list[2].ProcessId);
			Assert.AreEqual(3, list[3].ProcessId);
			Assert.AreEqual(4, list.Count);
		}
	}
}
