using System;

namespace ReAttach
{
    public static class ReAttachConstants
    {
        public const string ReAttachGuidString = "0ee94538-22b7-40c8-a253-3fce8ad39b6f";
        public static readonly Guid ReAttachPackageCmdSet = new Guid("3a680c5b-f815-414b-aa4a-0be57dadb1af");
        public const int ReAttachCommandId = 0x200;
        public const int ReAttachHistorySize = 10;
        public const int BuildBeforeReAttachCommandId = 0x240;

        public const string ReAttachRegistryKeyName = "ReAttach";
        public const string ReAttachRegistryHistoryKeyPrefix = "History";

        public static class Texts
        {
            public const string MenuItemPrefix = "ReAttach to ";
            public const string NoTargetsAvailable = "No ReAttach targets available";
        }
    }
}
