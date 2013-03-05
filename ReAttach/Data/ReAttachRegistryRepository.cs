
using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using ReAttach.Contracts;
using ReAttach.Misc;

namespace ReAttach.Data
{
	public class ReAttachRegistryRepository : IReAttachRepository
	{
		private readonly IReAttachPackage _package;

		public ReAttachRegistryRepository(IReAttachPackage package)
		{
			_package = package;
		}

		public bool SaveTargets(ReAttachTargetList targets)
		{
			try
			{
				var root = _package.OpenUserRegistryRoot();
				if (root == null)
				{
					_package.Reporter.ReportError("Unable to open user root registry key.");
					return false;
				}

				var subkey = root.CreateSubKey(ReAttachConstants.ReAttachRegistryKeyName);
				if (subkey == null)
				{
					_package.Reporter.ReportError("Unable to open/create ReAttach subkey.");
					root.Close();
					return false;
				}
				var index = 1;
				foreach (var target in targets)
				{
					var data = string.Format("{0}{1}{2}{3}{4}{5}{6}",
						target.ProcessPath, ReAttachConstants.ReAttachRegistrySplitChar,
						target.ProcessUser, ReAttachConstants.ReAttachRegistrySplitChar,
						target.ProcessId, ReAttachConstants.ReAttachRegistrySplitChar,
						target.ServerName);
					subkey.SetValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + index, data);
					index++;
				}

				// Clear old keys.
				for (var i = targets.Count + 1; i <= ReAttachConstants.ReAttachHistorySize; i++)
					subkey.DeleteValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + i, false);

				subkey.Close();
				root.Close();
				return true;
			}
			catch (Exception e)
			{
				_package.Reporter.ReportError(
					"Unable to save ReAttachTargetList using ReAttachRegistryRepository. Message: {0}", e.Message);
			}
			return false;
		}

		public ReAttachTargetList LoadTargets()
		{
			try
			{
				var root = _package.OpenUserRegistryRoot();
				if (root == null)
				{
					_package.Reporter.ReportError("Unable to open user root registry key.");
					return null;
				}

				var subkey = root.OpenSubKey(ReAttachConstants.ReAttachRegistryKeyName);
				if (subkey == null)
				{
					_package.Reporter.ReportWarning(
						"Unable to open ReAttach registry subkey. This might be the first time ReAttach is started.");
					root.Close();
					return null;
				}

				var targets = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);
				for (var i = 1; i < ReAttachConstants.ReAttachHistorySize; i++)
				{
					var value = subkey.GetValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + i) as string;
					if (value == null)
						continue;
					var tokens = value.Split(new[] {ReAttachConstants.ReAttachRegistrySplitChar});
					if (tokens.Length != 4)
						continue;
					int pid;
					int.TryParse(tokens[2], out pid);
					targets.AddLast(new ReAttachTarget(pid, tokens[0], tokens[1], tokens[3]));
				}
				subkey.Close();
				root.Close();
				return targets;
			}
			catch (Exception e)
			{
				_package.Reporter.ReportWarning(
					"Unable to load history. This might be first time ReAttach is started. Exception: {0}", e.Message);
			}
			return null;
		}

		public bool IsFirstLoad()
		{
			IRegistryKey root = null;
			try
			{
				root = _package.OpenUserRegistryRoot();
				if (root == null)
				{
					_package.Reporter.ReportError("Unable to open user root registry key.");
					return false;
				}
				var subkey = root.OpenSubKey(ReAttachConstants.ReAttachRegistryKeyName);
				return subkey == null; // Only if no errors occurs and the key isn't found we can be sure this is the first load.
			}
			catch (Exception e)
			{
				_package.Reporter.ReportWarning(
					"Unable to open ReAttach subkey. This might be first time ReAttach is started. Exception: {0}", e.Message);
			}
			finally
			{
				if (root != null)
					root.Close();
			}
			return false;
		}

	}
}
