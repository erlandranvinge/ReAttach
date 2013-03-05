using EnvDTE90;

namespace ReAttach.Extensions
{
	public static class ProcessExtensions
	{
		public static string GetUsername(this Process3 process)
		{
			var name = process.UserName;
			if (string.IsNullOrEmpty(name))
				return name;
			var start = name.LastIndexOf('[');
			return start != -1 ? name.Substring(0, start).TrimEnd() : name;
		}
	}
}
