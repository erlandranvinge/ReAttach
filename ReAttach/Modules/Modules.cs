using System;
using System.Collections.Concurrent;

namespace ReAttach.Modules
{
	public static class ModuleRepository
	{
		private static readonly ConcurrentDictionary<Type, object> Loaded = new ConcurrentDictionary<Type, object>();
		public static void Register<T>(T module) where T : class
		{
			if (module == null)
				throw new Exception("Module parameter is null.");

			var type = typeof(T);
			if (Loaded.ContainsKey(type))
				throw new Exception(string.Format("Module {0} already loaded.", type.FullName));
			Loaded[type] = module;
		}

		public static T Resolve<T>() 
		{
			var type = typeof(T);
			object result;
			if (!Loaded.TryGetValue(type, out result))
				throw new Exception(string.Format("Module {0} not loaded.", type.FullName));
			return (T) result;
		}
	}
}
