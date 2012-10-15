using System;
using System.Windows;
using System.Windows.Threading;
using ReAttach.Modules;

namespace ReAttach.Dialogs
{
	/// <summary>
	/// Interaction logic for ReAttachDialog.xaml
	/// </summary>
	public partial class ReAttachDialog
	{
		private DispatcherTimer _timer = new DispatcherTimer();
		private readonly ReAttachTarget _target;

		public ReAttachDialog(ReAttachTarget target)
		{
			InitializeComponent();
			ProcessName.Text = string.Format("{0} ({1}).", target.ProcessName, target.ProcessUser);
			_target = target;
			_timer.Tick += TimerOnTick;
			_timer.Interval = new TimeSpan(0, 0, 1);
			_timer.Start();
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			var timer = (DispatcherTimer)sender;
			timer.Stop();
			var debuggerModule = ModuleRepository.Resolve<DebuggerModule>();

			if (_target != null)
			{
				if (debuggerModule.TryReAttach(_target))
				{
					Close();
					return;
				}
			}

			DotsText.Text += ".";
			if (DotsText.Text.Length > 5)
				DotsText.Text = "";
			timer.Start();
		}

		public ReAttachDialog(string helpTopic) : base(helpTopic)
		{
			InitializeComponent();
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		protected override void OnClosed(EventArgs e)
		{
			if (_timer != null)
			{
				_timer.Stop();
				_timer = null;
			}
		}
	}
}
