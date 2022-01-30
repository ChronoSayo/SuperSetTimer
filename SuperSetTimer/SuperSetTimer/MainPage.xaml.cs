using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperSetTimer
{
    public partial class MainPage : ContentPage
    {
        private Timer _timer;
        private Stopwatch _stopWatch;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            _timer = new Timer();
            _timer.Elapsed += OnTimerTick;
            _timer.Interval = 100;
            _timer.Enabled = false;
            _timer.AutoReset = true;

            _stopWatch = new Stopwatch();
            TimerLabel.Text = "0.0";
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            int setTimer = int.Parse(SecondsEntry.Text);
            TimeSpan remainingTimer = TimeSpan.FromSeconds(setTimer - _stopWatch.Elapsed.Seconds);
            MainThread.BeginInvokeOnMainThread(() => { TimerLabel.Text = remainingTimer.ToString(@"m\:ss"); });
        }

        private void OnTimerStart(object sender, EventArgs e)
        {
            _stopWatch.Start();
            _timer.Enabled = true;
            _timer.Start();
        }
        private void OnTimerStop(object sender, EventArgs e)
        {
            _stopWatch.Stop();
            _timer.Enabled = false;
            _timer.Stop();
        }
    }
}
