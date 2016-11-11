using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ReAttach
{
	[ExcludeFromCodeCoverage]
	public static class ReAttachConstants
	{
		public const string ReAttachPackageGuidString = "0ee94538-22b7-40c8-a253-3fce8ad39b6f";

		// Settings.
		public const string ReAttachRegistryKeyName = "ReAttach";
		public const string ReAttachRegistryHistoryKeyPrefix = "History";
        public const string ReAttachRegistryOptionsKeyName = "Options";

		// Commands.
		public static readonly Guid ReAttachPackageCmdSet = new Guid("3a680c5b-f815-414b-aa4a-0be57dadb1af");
		public const int ReAttachCommandId = 0x200;
		public const int ReAttachHistorySize = 5;
        public const int BuildBeforeReAttachCommandId = 0x240;

		// Key bindings (used by DTE, not used right now).
		public const string ReAttachToLastCommandName = "Debug.ReAttach";
		public const string ReAttachToLastKeyBinding = "Global::Ctrl+R, Ctrl+A";

        public static readonly HashSet<Guid> IgnoredDebuggingEngines = new HashSet<Guid> { 
            new Guid("2c18241e-069a-43b2-bd81-89c186af994b") // IntelliTrace
        };

		// Texts
		public static class Texts
		{
			public const string MenuItemPrefix = "ReAttach to ";
			public const string NoTargetsAvailable = "No ReAttach targets available";
		}
	};
}