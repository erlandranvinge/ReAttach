
namespace ReAttach.Contracts
{
	public interface IReAttachReporter
	{
		int ErrorCount { get; }
		int WarningCount { get; }

		void ReportError(string message, params object[] args);
		void ReportWarning(string message, params object[] args);
        void ReportTrace(string message, params object[] args);
	}
}
