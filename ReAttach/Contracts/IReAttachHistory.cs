using ReAttach.Data;

namespace ReAttach.Contracts
{
	public interface IReAttachHistory
	{
		ReAttachTargetList Items { get; }
        ReAttachOptions Options { get; }
		bool Load();
		bool Save();
	}
}
