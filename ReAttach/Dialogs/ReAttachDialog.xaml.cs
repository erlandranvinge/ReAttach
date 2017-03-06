using System;
using System.Windows;
using System.Windows.Threading;
using ReAttach.Contracts;
using ReAttach.Data;

namespace ReAttach.Dialogs
{
	public partial class ReAttachDialog 
	{
		private readonly IReAttachPackage _package;
		private readonly ReAttachTarget _target;

		private DispatcherTimer _timer = new DispatcherTimer();
		private int _progress = 0;

		public ReAttachDialog(IReAttachPackage package, ReAttachTarget target)
		{
			InitializeComponent();
			_package = package;
			_target = target;

			var name = target.ProcessName;
			if (name.Length > 50) name = name.Substring(0, 50);

			ProcessName.Text = name;
			Dots.Text = "";
			_timer.Tick += TimerOnTick;
			_timer.Interval = new TimeSpan(0, 0, 1);
			_timer.Start();
		}

		public ReAttachDialog(string helpTopic)
			: base(helpTopic)
		{
			InitializeComponent();
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			var timer = (DispatcherTimer)sender;
			timer.Stop();

			if (_package.Debugger.ReAttach(_target))
			{
				Close();
				return;
			}
			_progress++;
			if (_progress > 5)
			{
				Dots.Text = "";
				_progress = 0;
			}
			else
				Dots.Text += '.';
			timer.Start();
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		protected override void OnClosed(EventArgs e)
		{
			if (_timer == null) 
				return;
			_timer.Stop();
			_timer = null;
		}
	}
}
