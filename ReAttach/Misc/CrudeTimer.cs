using System.Diagnostics;

namespace ReAttach.Misc
{
	public static class CrudeTimer
	{
		private static readonly Stopwatch Watch = new Stopwatch();

		public static void Start()
		{
			Watch.Start();
		}

		public static void Stop()
		{
			var timeTaken = Watch.ElapsedMilliseconds;
			Watch.Reset();
			Trace.WriteLine("Time taken: " + timeTaken + " ms");
		}
	}
}
