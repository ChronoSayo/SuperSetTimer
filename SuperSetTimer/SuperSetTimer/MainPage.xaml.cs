using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperSetTimer
{
    public partial class MainPage : ContentPage
    {
        private readonly Timer _timer;
        private readonly Stopwatch _stopWatch;
        private int _sets;
        private bool _isCooldown, _firstSet;

        private int Sets => int.Parse(SetsEntry.Text);

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            _timer = new Timer();
            _timer.Elapsed += OnTimerTick;
            _timer.Interval = 1;
            _timer.Enabled = false;
            _timer.AutoReset = true;

            _isCooldown = false;
            _firstSet = true;

            _stopWatch = new Stopwatch();
            TimerLabel.Text = "0.0";
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            int setTimer;
            if (_firstSet)
                setTimer = 2;
            else
                setTimer = _isCooldown ? int.Parse(CooldownEntry.Text) : int.Parse(ActiveEntry.Text);
            TimeSpan remainingTimer = TimeSpan.FromSeconds(setTimer - _stopWatch.Elapsed.Seconds);

            int showSetsLeft = Sets - _sets;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerLabel.Text = "Sets: " + showSetsLeft + "/" + SetsEntry.Text + "\n";
                TimerLabel.Text += remainingTimer.ToString(@"m\:ss");
            });

            if (remainingTimer.Seconds < 0)
            {
                if (_firstSet)
                    _firstSet = false;

                _isCooldown = !_isCooldown;
                if (!_isCooldown)
                {
                    _sets--;
                    StatusLabel.Text = "GO!";
                }
                else
                    StatusLabel.Text = "Cooldown";

                _stopWatch.Restart();
            }

            if (_sets >= 0) 
                return;

            _timer.Stop();
            EnableEntries(true);
            _stopWatch.Restart();
        }

        private void OnTimerStart(object sender, EventArgs e)
        {
            _stopWatch.Start();
            _timer.Enabled = true;
            _timer.Start();
            _isCooldown = true;
            _sets = int.Parse(SetsEntry.Text);
            _firstSet = true;
            StatusLabel.Text = "Get ready!";

            EnableEntries(false);

        }
        private void OnTimerStop(object sender, EventArgs e)
        {
            _stopWatch.Stop();
            _timer.Enabled = false;
            _timer.Stop();
            StatusLabel.Text = "Paused";
        }
        private void OnTimerReset(object sender, EventArgs e)
        {
            _stopWatch.Stop();
            _stopWatch.Reset();
            _timer.Enabled = false;
            _timer.Stop();
            _firstSet = true;
            StatusLabel.Text = "Starting over.";

            EnableEntries(true);
        }

        private void EnableEntries(bool enable)
        {
            CooldownEntry.IsEnabled = enable;
            ActiveEntry.IsEnabled = enable;
            SetsEntry.IsEnabled = enable;
        }
    }
}
