using System;
using System.Collections.Generic;
using System.IO;

namespace ReAttach.Models
{
	public class ReAttachTarget
	{
		public int ProcessId { get; set; }
		public string ProcessName { get; set; }
		public string ProcessPath { get; set; }
		public string ProcessUser { get; set; }
		public string ServerName { get; set; }
		public bool IsAttached { get; set; }
		public bool IsLocal { get { return string.IsNullOrEmpty(ServerName); } }
		public List<Guid> Engines { get; set; }

		public ReAttachTarget(int pid, string path, string user, string serverName = "")
		{
			try
			{
				ProcessName = Sanitize(Path.GetFileName(path));
			}
			catch
			{
				ProcessName = Sanitize(path);
			}
			ProcessId = pid;
			ProcessPath = Sanitize(path);
			ProcessUser = user ?? "";
			ServerName = serverName ?? "";
			Engines = new List<Guid>();
		}

        public ReAttachTarget() { }

		public override bool Equals(object obj)
		{
			var other = obj as ReAttachTarget;
			if (other == null)
				return false;
			return ProcessPath.Equals(other.ProcessPath, StringComparison.OrdinalIgnoreCase) &&
				ProcessUser.Equals(other.ProcessUser, StringComparison.OrdinalIgnoreCase) &&
				ServerName.Equals(other.ServerName, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return ProcessPath.ToLower().GetHashCode() +
				ProcessUser.ToLower().GetHashCode() +
				ServerName.ToLower().GetHashCode();
		}

		public override string ToString()
		{
			return IsLocal ?
				string.Format("{0} ({1})", ProcessName, ProcessUser) :
				string.Format("{0} ({1}@{2})", ProcessName, ProcessUser, ServerName);
		}

		public static string Sanitize(string str)
		{
			if (string.IsNullOrEmpty(str)) return "";
			return str.Replace(".vshost", "");
		}
	}
}