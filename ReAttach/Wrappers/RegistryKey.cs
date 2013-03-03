using System.Diagnostics.CodeAnalysis;
using Win32RegistryKey = Microsoft.Win32.RegistryKey;

namespace ReAttach.Wrappers
{
	[ExcludeFromCodeCoverage]
	public class RegistryKey : IRegistryKey
	{
		private readonly Win32RegistryKey _key;

		public RegistryKey(Win32RegistryKey key)
		{
			_key = key;
		}

		public IRegistryKey CreateSubKey(string subkey)
		{
			return _key == null ? null : new RegistryKey(_key.CreateSubKey(subkey));
		}

		public IRegistryKey OpenSubKey(string name)
		{
			return _key == null ? null : new RegistryKey(_key.OpenSubKey(name));
		}

		public object GetValue(string name)
		{
			return _key == null ? null : _key.GetValue(name);
		}

		public void SetValue(string name, object value)
		{
			if (_key == null)
				return;
			_key.SetValue(name, value);
		}

		public void DeleteValue(string name, bool throwOnMissingSubKey)
		{
			if (_key == null)
				return;
			_key.DeleteValue(name, throwOnMissingSubKey);
		}

		public void Close()
		{
			if (_key == null)
				return;
			_key.Close();
		}
	}
}
