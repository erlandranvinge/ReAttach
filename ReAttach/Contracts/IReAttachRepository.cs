using ReAttach.Data;

namespace ReAttach.Contracts
{
	public interface IReAttachRepository
	{
		bool Save(ReAttachTargetList targets);
		ReAttachTargetList Load();
	}
}
