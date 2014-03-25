using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReAttach.Data
{
	public class ReAttachSettings
	{
		public bool AutoReAttachEnabled { get; set; }

		public ReAttachSettings()
		{
		 	AutoReAttachEnabled = true; // For now.
		}
	}
}
