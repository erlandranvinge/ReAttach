using System;
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
		public const char ReAttachRegistrySplitChar = ';';

		// Commands.
		public static readonly Guid ReAttachPackageCmdSet = new Guid("3a680c5b-f815-414b-aa4a-0be57dadb1af");
		public const int ReAttachCommandId = 0x200;
		public const int ReAttachHistorySize = 5;

		// Key bindings (used by DTE). Which is not used right now.
		public const string ReAttachToLastCommandName = "Debug.ReAttach";
		public const string ReAttachToLastKeyBinding = "Global::Ctrl+R, Ctrl+A";

		// Texts
		public static class Texts
		{
			public const string MenuItemPrefix = "ReAttach to ";
			public const string NoTargetsAvailable = "No ReAttach targets available";
		}
	};
}