using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ReAttach.Modules
{
	public class ReAttachTarget 
	{
		public int ProcessId { get; private set; }
		public string ProcessName { get; private set; }
		public string ProcessPath { get; private set; }
		public string ProcessUser { get; private set; }

		public ReAttachTarget(string path, string user)
		{
			ProcessName = Path.GetFileName(path);
			ProcessPath = path;
			ProcessUser = user;
		}

		public ReAttachTarget(int pid, string path, string user) : this(path, user)
		{
			ProcessId = pid;
		}

		public bool IsValid()
		{
			return !string.IsNullOrEmpty(ProcessPath);
		}

		public override bool Equals(object obj)
		{
			var other = obj as ReAttachTarget;
			if (other == null)
				return false;

			return
				ProcessPath.Equals(other.ProcessPath, StringComparison.OrdinalIgnoreCase) && 
				ProcessUser.Equals(other.ProcessUser, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return ProcessPath.GetHashCode() + ProcessUser.GetHashCode();
		}
	}

	public class ReAttachTargets
	{
		public List<ReAttachTarget> Items = new List<ReAttachTarget>();

		public void AddItem(ReAttachTarget entry, bool persist = true)
		{
			Items.Remove(entry);
			Items.Insert(0, entry);
			if (Items.Count > Constants.ReAttachHistorySize)
				Items.RemoveAt(Constants.ReAttachHistorySize);

			if (persist)
				Save();
		}

		public ReAttachTarget TryGetItem(int index)
		{
			if (index < 0 || index >= Items.Count)
				return null;

			var item = Items[index];
			return item.IsValid() ? item : null;
		}

		public void Load()
		{
			var package = ModuleRepository.Resolve<ReAttachPackage>();
			var root = package.UserRegistryRoot;

			var subkey = root.OpenSubKey(Constants.ReAttachRegistryKeyName);
			if (subkey == null)
			{
				root.Close();
				return;
			}

			Items.Clear();
			for (int i = 1; i < Constants.ReAttachHistorySize; i++)
			{
				var value = subkey.GetValue(Constants.ReAttachRegistryHistoryKeyPrefix + i) as string;
				if (value == null)
					break;

				var tokens = value.Split(Constants.ReAttachRegistrySplitChar);
				if (tokens.Length != 3 || string.IsNullOrEmpty(tokens[0]) || string.IsNullOrEmpty(tokens[1]))
					break;

				int pid = 0;
				int.TryParse(tokens[2], out pid);
				var target = new ReAttachTarget(pid, tokens[0], tokens[1]);
				if (!target.IsValid())
					break;

				Items.Add(target);
			}
			subkey.Close();
			root.Close();
		}

		public void Save()
		{
			var package = ModuleRepository.Resolve<ReAttachPackage>();
			var root = package.UserRegistryRoot;
			var subkey = root.CreateSubKey(Constants.ReAttachRegistryKeyName);
			if (subkey == null)
			{
				root.Close();
				return;
			}

			int i = 1;
			foreach (var item in Items)
			{
				var itemData = string.Format("{0}{1}{2}{3}{4}",
					item.ProcessPath, Constants.ReAttachRegistrySplitChar,
					item.ProcessUser, Constants.ReAttachRegistrySplitChar,
					item.ProcessId);
				subkey.SetValue(Constants.ReAttachRegistryHistoryKeyPrefix + i, itemData);
				i++;
			}
			subkey.Close();
			root.Close();
		}
	}
}
