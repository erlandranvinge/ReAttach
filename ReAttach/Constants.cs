using System;

namespace ReAttach
{
    static class Constants
    {
        public const string ReAttachPackageGuidString = "0ee94538-22b7-40c8-a253-3fce8ad39b6f";

		// Settings.
	    public const string ReAttachRegistryKeyName = "ReAttach";
	    public const string ReAttachRegistryHistoryKeyPrefix = "History";
	    public const char ReAttachRegistrySplitChar = ';';

		// Commands.
		public static readonly Guid ReAttachPackageCmdSet = new Guid("3a680c5b-f815-414b-aa4a-0be57dadb1af");
		public const int ReAttachCommandId = 0x200;
	    public const int ReAttachMenuCommandId = 0x100;
		public const int ReAttachHistorySize = 5;
    };
}