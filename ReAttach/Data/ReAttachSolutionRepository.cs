using ReAttach.Contracts;

namespace ReAttach.Data
{
	public class ReAttachSolutionRepository : IReAttachRepository
	{
		public bool Save(ReAttachTargetList targets)
		{
			return false;
		}

		public ReAttachTargetList Load()
		{
			return null;
		}
	}
}
