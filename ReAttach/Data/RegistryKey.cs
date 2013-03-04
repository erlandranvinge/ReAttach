using System.Diagnostics.CodeAnalysis;
using ReAttach.Contracts;

namespace ReAttach.Data
{
	[ExcludeFromCodeCoverage]
	public class RegistryKey : IRegistryKey
	{
		private readonly Microsoft.Win32.RegistryKey _key;

		public RegistryKey(Microsoft.Win32.RegistryKey key)
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
