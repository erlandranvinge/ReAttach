using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ReAttach.Data;

namespace ReAttach.Modules
{
	public class HistoryModule : IEnumerable<ReAttachTarget>
	{
		public ReAttachTargetList Targets { get; private set; }

		public HistoryModule()
		{
			Targets = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);
		}
		
		public bool Load()
		{
			var package = ModuleRepository.Resolve<ReAttachPackage>();
			try
			{
				var root = package.UserRegistryRoot;
				var subkey = root.OpenSubKey(ReAttachConstants.ReAttachRegistryKeyName);
				if (subkey == null)
				{
					Trace.WriteLine("ReAttach: Unable to open ReAttach subkey.");
					root.Close();
					return false;
				}

				var targets = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);
				for (var i = 1; i < ReAttachConstants.ReAttachHistorySize; i++)
				{
					var value = subkey.GetValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + i) as string;
					if (value == null)
						break;
					var tokens = value.Split(new[] { ReAttachConstants.ReAttachRegistrySplitChar }, 
						StringSplitOptions.RemoveEmptyEntries);

					if (tokens.Length != 3)
						break;

					int pid;
					int.TryParse(tokens[2], out pid);
					targets.AddLast(new ReAttachTarget(pid, tokens[0], tokens[1]));
				}
				Targets = targets;
				return true;
			} catch
			{
				Trace.WriteLine("ReAttach: Unable to load history. This might be first time ReAttach is started.");
			}
			return false;
		}
	
		public bool Save()
		{
			var package = ModuleRepository.Resolve<ReAttachPackage>();
			var root = package.UserRegistryRoot;
			try
			{
				var subkey = root.CreateSubKey(ReAttachConstants.ReAttachRegistryKeyName);
				if (subkey == null)
				{
					Trace.WriteLine("ReAttach: Unable to open/create ReAttach subkey.");
					root.Close();
					return false;
				}
				var index = 1;
				foreach (var target in Targets)
				{
					var data = string.Format("{0}{1}{2}{3}{4}",
						target.ProcessPath, ReAttachConstants.ReAttachRegistrySplitChar,
						target.ProcessUser, ReAttachConstants.ReAttachRegistrySplitChar,
						target.ProcessId);
					subkey.SetValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + index, data);
					index++;
				}
				
				// Clear old keys.
				for (var i = Targets.Count + 1; i <= ReAttachConstants.ReAttachHistorySize; i++)
					subkey.DeleteValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + i, false);

				subkey.Close();
				root.Close();
				return true;
			}
			catch
			{
				Trace.WriteLine("ReAttach: Unable to save history. ");
			}
			return false;
		}

		public IEnumerator<ReAttachTarget> GetEnumerator()
		{
			return Targets.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
