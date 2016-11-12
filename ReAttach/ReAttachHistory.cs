using ReAttach.Contracts;
using ReAttach.Data;

namespace ReAttach
{
	public class ReAttachHistory : IReAttachHistory
	{
		private readonly IReAttachRepository _repository;

		public ReAttachTargetList Items { get; private set; }
        public ReAttachOptions Options { get; private set; }

		public ReAttachHistory(IReAttachRepository repository)
		{
			_repository = repository;
			Items = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);
            Options = new ReAttachOptions();
		}

		// TODO: Locking on save/loads might be relevant. Ref-switching will do for now.
		public bool Load()
		{
			var items = _repository.LoadTargets();
			if (items == null)
				return false;

			Items = items;
			return true;
		}

		public bool Save()
		{
			return _repository.SaveTargets(Items);
		}

        public void Clear()
        {
            Items.Clear();
            Save();
        }
	}
}
