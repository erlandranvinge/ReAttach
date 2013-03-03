
namespace ReAttach.Wrappers
{
	public interface IRegistryKey
	{
		IRegistryKey CreateSubKey(string reAttachRegistryKeyName);
		IRegistryKey OpenSubKey(string name);
		object GetValue(string name);
		void SetValue(string name, object value);
		void DeleteValue(string name, bool throwOnMissingSubKey);
		void Close();
	}
}
