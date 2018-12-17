using System;
using System.Threading.Tasks;

namespace ReAttach.Contracts
{
	public interface IReAttachUi
	{
		void Update();
		Task ReAttachCommandClickedAsync(object sender, EventArgs e);
		Task MessageBoxAsync(string message);
	}
}
