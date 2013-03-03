using System;

namespace ReAttach.Contracts
{
	public interface IReAttachUi
	{
		void Update();
		void ReAttachCommandClicked(object sender, EventArgs e);
		void MessageBox(string message);
	}
}
