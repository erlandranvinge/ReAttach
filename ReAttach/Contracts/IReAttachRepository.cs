using ReAttach.Data;

namespace ReAttach.Contracts
{
	public interface IReAttachRepository
	{
		bool SaveTargets(ReAttachTargetList targets);
		ReAttachTargetList LoadTargets();
		bool IsFirstLoad();
	}
}
