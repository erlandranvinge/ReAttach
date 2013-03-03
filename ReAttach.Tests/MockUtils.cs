using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;

namespace ReAttach.Tests
{
	[ExcludeFromCodeCoverage]
	internal static class MockUtils
	{
		public static Mock<T> GetMockFromObject<T>(T mockedObject) where T : class
		{
			var pis = mockedObject.GetType().GetProperties()
				.Where(p => p.PropertyType.Name == "Mock`1").ToArray();
			return pis.First().GetGetMethod().Invoke(mockedObject, null) as Mock<T>;
		}
	}
}
