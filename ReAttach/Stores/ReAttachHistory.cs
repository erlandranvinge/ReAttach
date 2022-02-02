using Newtonsoft.Json;
using ReAttach.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReAttach.Stores
{
    public class ReAttachHistory
    {
        private readonly ReAttachTargetList _targets = new ReAttachTargetList(ReAttachConstants.ReAttachHistorySize);
        private readonly ReAttachPackage _package;

        public ReAttachHistory(ReAttachPackage package)
        {
            _package = package;
            Load();
        }

        public void AddFirst(ReAttachTarget target) => _targets.AddFirst(target);

        public ReAttachTarget FindByPid(int pid) => _targets.Find(pid);

        public ReAttachTarget[] GetUnAttached() => _targets.Where(t => !t.IsAttached).ToArray();

        public void Load()
        {
            var root = _package.UserRegistryRoot;
            if (root == null)
                return;

            var parent = root.OpenSubKey(ReAttachConstants.ReAttachRegistryKeyName);
            if (parent == null) // First start?
            {
                root.Close();
                return;
            }

            var targets = new List<ReAttachTarget>();
            for (var i = 1; i < ReAttachConstants.ReAttachHistorySize; i++)
            {
                var json = parent.GetValue(ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + i) as string;
                if (json == null)
                    continue;
                try
                {
                    var target = JsonConvert.DeserializeObject<ReAttachTarget>(json);
                    if (target != null)
                        targets.Add(target);
                }
                catch (Exception) { /* Ignore broken elements */}
            }
            parent.Close();
            root.Close();
            _targets.Set(targets);
        }

        public void Save()
        {
            var root = _package.UserRegistryRoot;
            if (root == null)
            {
                ReAttachUtils.ShowError("ReAttach save failed.", "Unable to open root key.");
                return;
            }

            var parent = root.CreateSubKey(ReAttachConstants.ReAttachRegistryKeyName);
            if (parent == null)
            {
                ReAttachUtils.ShowError("ReAttach save failed.", "Unable to open parent key.");
                root.Close();
                return;
            }

            for (var i = 1; i <= ReAttachConstants.ReAttachHistorySize; i++)
            {
                var key = ReAttachConstants.ReAttachRegistryHistoryKeyPrefix + i;
                if (i <= _targets.Count)
                {
                    var target = _targets[i - 1];
                    var json = JsonConvert.SerializeObject(target);
                    parent.SetValue(key, json);
                }
                else
                {
                    parent.DeleteValue(key, false);
                }
            }
            parent.Close();
            root.Close();
        }

        public void Clear()
        {
            _targets.Clear();
            Save();
        }

    }
}
