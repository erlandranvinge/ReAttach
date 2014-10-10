
using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Newtonsoft.Json;
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
					var json = JsonConvert.SerializeObject(target);
					subkey.SetValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + index, json);
					index++;
				}

				subkey.Close();
				root.Close();
				return true;
			}
			catch (Exception e)
			{
				_package.Reporter.ReportError(
					"Unable to save ReAttachTargetList using ReAttachRegistryRepository. Message: {0}", e.Message);
				return false;
			}
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
					var json = subkey.GetValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + i) as string;
					if (json == null)
						continue;
					try
					{
						var target = JsonConvert.DeserializeObject<ReAttachTarget>(json);
						targets.AddLast(target);
					} catch (Exception e) { /* Ignore broken elements */}
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
	}
}
