using System.Diagnostics;
using ReAttach.Contracts;

namespace ReAttach
{
	public class ReAttachTraceReporter : IReAttachReporter
	{
		public int ErrorCount { get; private set; }
		public int WarningCount { get; private set; }

		public void ReportError(string message, params object[] args)
		{
			Trace.WriteLine("ReAttach Error: " + string.Format(message, args));
			ErrorCount++;
		}

		public void ReportWarning(string message, params object[] args)
		{
			Trace.WriteLine("ReAttach Warning: " + string.Format(message, args));
			WarningCount++;
		}

		public void ReportTrace(string message, params object[] args)
		{
			Trace.WriteLine(string.Format(message, args));
			WarningCount++;
		}

	}
}
