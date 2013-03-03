using ReAttach.Contracts;
using ReAttach.Data;

namespace ReAttach
{
	public class ReAttachHistory : IReAttachHistory
	{
		private readonly IReAttachRepository _repository;
		public ReAttachTargetList Items { get; private set; }

		public ReAttachHistory(IReAttachRepository repository)
		{
			_repository = repository;
			Items = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);
		}

		// TODO: Locking on save/loads might be relevant. Ref-switching will do for now.
		public bool Load()
		{
			var items = _repository.Load();
			if (items == null)
				return false;

			Items = items;
			return true;
		}

		public bool Save()
		{
			return _repository.Save(Items);
		}
	}
}
