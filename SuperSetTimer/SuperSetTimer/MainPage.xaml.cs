using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        private bool _isCooldown, _startUp, _paused;

        private int StartUpTime => int.Parse(StartUpEntry.Text);
        private int ActiveTime => int.Parse(ActiveEntry.Text);
        private int CooldownTime => int.Parse(CooldownEntry.Text);
        private int Sets => int.Parse(SetsEntry.Text);

        private string TimerText
        {
            get => TimerLabel.Text;
            set => TimerLabel.Text = value;
        }
        private string StatusText
        {
            set => StatusLabel.Text = value;
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            _timer = new Timer();
            _timer.Elapsed += OnTimerTick;
            _timer.Interval = 10;
            _timer.Enabled = false;
            _timer.AutoReset = true;

            _isCooldown = false;
            _startUp = true;
            _paused = false;

            _stopWatch = new Stopwatch();
            TimerText = "0.0";
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            int setTimer;
            if (_startUp)
                setTimer = StartUpTime;
            else
                setTimer = _isCooldown ? CooldownTime : ActiveTime;
            TimeSpan remainingTimer = TimeSpan.FromSeconds(setTimer) - _stopWatch.Elapsed;
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerText = "Sets: " + _sets + "/" + Sets + "\n";
                TimerText += remainingTimer.ToString(@"m\:ss\.ff");
            });

            if (remainingTimer.Seconds >= 0 && remainingTimer.Milliseconds >= 0)
                return;

            _isCooldown = !_isCooldown;

            if (_startUp)
            {
                _startUp = false;
                _stopWatch.Restart();
                return;
            }

            if (!_isCooldown)
            {
                _sets++; 
                StatusText = "GO!";
            }
            else
            {
                if (_sets > Sets)
                {
                    StatusText = "DONE!";
                    EnableEntries(true);
                    Reset();
                    return;
                }
                StatusText = "Rest";
            }

            _stopWatch.Restart();
        }

        private void OnTimerStart(object sender, EventArgs e)
        {
            _stopWatch.Start();
            _timer.Enabled = true;
            _timer.Start();
            if (!_paused)
            {
                _isCooldown = true;
                _startUp = true;
                _sets = 0;
                StatusText = "Get ready!";
            }
            else
                _paused = false;

            EnableEntries(false);

        }
        private void OnTimerStop(object sender, EventArgs e)
        {
            _stopWatch.Stop();
            _timer.Enabled = false;
            _timer.Stop();
            StatusText = "Paused";
            _paused = true;
        }
        private void OnTimerReset(object sender, EventArgs e)
        {
            StatusText = "Starting over.";
            Reset();
        }

        private void EnableEntries(bool enable)
        {
            CooldownEntry.IsEnabled = enable;
            ActiveEntry.IsEnabled = enable;
            SetsEntry.IsEnabled = enable;
            StartUpEntry.IsEnabled = enable;
        }

        private void Reset()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();

            _timer.Enabled = false;
            _timer.Stop();

            _startUp = true;
            TimerText = "0.0";
            EnableEntries(true);
        }
    }
}
