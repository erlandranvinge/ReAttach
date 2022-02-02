using ReAttach.Models;
using ReAttach.Services;
using System;
using System.Windows;
using System.Windows.Threading;
namespace ReAttach.Dialogs
{
	public partial class WaitingDialog
	{
		private readonly ReAttachDebugger _debugger;
		private readonly ReAttachTarget _target;

		private DispatcherTimer _timer = new DispatcherTimer();
		private int _progress = 0;

		public ReAttachResult Result { get; private set; }

		public WaitingDialog(ReAttachDebugger debugger, ReAttachTarget target)
		{
			_debugger = debugger;
			_target = target;

			InitializeComponent();

			var name = target.ProcessName;
			if (name.Length > 50) name = name.Substring(0, 50);

			ProcessName.Text = name;
			Dots.Text = "";
			_timer.Tick += TimerOnTick;
			_timer.Interval = new TimeSpan(0, 0, 1);
			_timer.Start();
		}

		public WaitingDialog(string helpTopic)
			: base(helpTopic)
		{
			InitializeComponent();
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			var timer = (DispatcherTimer)sender;
			timer.Stop();

			Result = _debugger.ReAttach(_target);
			
			if (Result != ReAttachResult.NotStarted)
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
			Result = ReAttachResult.Failed;
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