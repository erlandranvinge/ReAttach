using ReAttach.Data;

namespace ReAttach.Contracts
{
	public interface IReAttachHistory
	{
		ReAttachTargetList Items { get; }
		bool Load();
		bool Save();
	}
}
