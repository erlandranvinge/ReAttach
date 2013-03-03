using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ReAttach.Data
{
	public sealed class ReAttachTargetList : IEnumerable<ReAttachTarget>
	{
		private readonly List<ReAttachTarget> _targets = new List<ReAttachTarget>();

		private readonly int _maxItems;

		public ReAttachTargetList(int maxItems)
		{
			_maxItems = maxItems;
		}

		public bool IsEmpty
		{
			get { return _targets.Count == 0; }
		}

		public int Count
		{
			get { return _targets.Count; }
		}

		public ReAttachTarget this[int index]
		{
			get { return index >= 0 && index < _targets.Count ? _targets[index] : null; }
		}

		public void AddFirst(ReAttachTarget target)
		{
			var pos = _targets.IndexOf(target);
			if (pos == 0)
			{
				_targets[0] = target;
				return;
			}
			if (pos >= 0)
				_targets.RemoveAt(pos);
			else
			{
				if (_targets.Count == _maxItems)
					_targets.RemoveAt(_targets.Count - 1);
			}
			_targets.Insert(0, target);
		}

		public void AddLast(ReAttachTarget target)
		{
			var pos = _targets.IndexOf(target);
			if (pos >= 0 && pos == _targets.Count - 1)
			{
				_targets[_targets.Count - 1] = target;
				return;
			}
			if (pos >= 0)
				_targets.RemoveAt(pos);
			else
			{
				if (_targets.Count == _maxItems)
					_targets.RemoveAt(_targets.Count - 1);
			}
			_targets.Add(target);
		}

		public ReAttachTarget Find(int pid)
		{
			return _targets.Find(p => p.ProcessId == pid);
		}

		public ReAttachTarget Find(string path, string user, string serverName)
		{
			return _targets.Find(p =>
				p.ProcessPath.Equals(path, StringComparison.OrdinalIgnoreCase) &&
				p.ProcessUser.Equals(user, StringComparison.OrdinalIgnoreCase) &&
				p.ServerName.Equals(serverName, StringComparison.OrdinalIgnoreCase));
		}

		public void Clear()
		{
			_targets.Clear();
		}

		public IEnumerator<ReAttachTarget> GetEnumerator()
		{
			return _targets.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
