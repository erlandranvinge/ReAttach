using System;

namespace ReAttach.Services
{
    public class ReAttachOptions
    {
        private ReAttachPackage _package;
        private bool _buildBeforeReAttach = false;
        private const string BuildBeforeReAttachKey = "BuildBeforeReAttach";

        public ReAttachOptions(ReAttachPackage package)
        {
            _package = package;
            Load();
        }

        public bool BuildBeforeReAttach
        {
            get => _buildBeforeReAttach;
            set
            {
                _buildBeforeReAttach = value;
                Save();
            }
        }

        private void Load()
        {
            var root = _package.UserRegistryRoot;
            if (root == null)
                return;

            var parent = root.OpenSubKey(ReAttachConstants.ReAttachRegistryKeyName);
            if (parent == null)
            {
                root.Close();
                return;
            }

            var value = parent.GetValue(BuildBeforeReAttachKey, false);
            if (value == null)
                return;

            try
            {
                _buildBeforeReAttach = Convert.ToBoolean(value);
            } catch { /* don't care */ }

            parent.Close();
            root.Close();
        }

        private void Save()
        {
            var root = _package.UserRegistryRoot;
            if (root == null)
                return;

            var parent = root.CreateSubKey(ReAttachConstants.ReAttachRegistryKeyName);
            if (parent == null)
            {
                root.Close();
                return;
            }

            try
            {
                parent.SetValue(BuildBeforeReAttachKey, _buildBeforeReAttach.ToString());
            } catch { /* don't care */ }

            parent.Close();
            root.Close();

        }
    }
}
