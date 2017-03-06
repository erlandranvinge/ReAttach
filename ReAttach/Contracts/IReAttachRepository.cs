using ReAttach.Data;

namespace ReAttach.Contracts
{
	public interface IReAttachRepository
	{
        ReAttachTargetList LoadTargets();
		bool SaveTargets(ReAttachTargetList targets);
        bool ClearTargets();
	}
}
