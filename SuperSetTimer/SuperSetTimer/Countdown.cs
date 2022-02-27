using System;
using System.Diagnostics;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperSetTimer
{
    class Countdown
    {
        private readonly Timer _timer;
        private readonly Stopwatch _stopWatch;
        private int _sets;
        private bool _isCooldown, _startUp, _paused;

        private enum State
        {
            StandBy, StartUp, Active, Cooldown, Paused
        }

        public Entry StartUpEntry { get; set; }
        public Entry ActiveEntry { get; set; }
        public Entry CooldownEntry { get; set; }
        public Entry SetsEntry { get; set; }
        public Label TimerLabel { get; set; }
        public Label StatusLabel { get; set; }
        public Frame StatusFrame { get; set; }

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

        public Countdown()
        {
            _timer = new Timer();
            _timer.Elapsed += OnTimerTick;
            _timer.Interval = 1;
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
            MainThread.BeginInvokeOnMainThread(() =>
            {
                int setTimer;
                if (_startUp)
                    setTimer = StartUpTime;
                else
                    setTimer = _isCooldown ? CooldownTime : ActiveTime;
                TimeSpan remainingTimer = TimeSpan.FromSeconds(setTimer) - _stopWatch.Elapsed;

                TimerText = "Sets: " + _sets + "/" + Sets + "\n";
                TimerText += remainingTimer.ToString(@"m\:ss\.ff");

                if (remainingTimer.Seconds >= 0 && remainingTimer.Milliseconds >= 0)
                    return;

                _isCooldown = !_isCooldown;

                if (_startUp)
                    _startUp = false;

                if (!_isCooldown)
                {
                    _sets++;
                    SetVisualsByState(State.Active);
                }
                else
                {
                    if (_sets >= Sets)
                    {
                        SetVisualsByState(State.StandBy);
                        EnableEntries(true);
                        Reset();
                        return;
                    }
                    SetVisualsByState(State.Cooldown);
                }

                _stopWatch.Restart();
            });
        }

        public void Start()
        {
            _stopWatch.Start();
            _timer.Enabled = true;
            _timer.Start();
            if (!_paused)
            {
                _isCooldown = true;
                _startUp = true;
                _sets = 0;
                SetVisualsByState(State.Active);
            }
            else
                _paused = false;

            EnableEntries(false);

        }
        public void Stop()
        {
            _stopWatch.Stop();
            _timer.Enabled = false;
            _timer.Stop();
            StatusText = "Paused";
            _paused = true;
            SetVisualsByState(State.StandBy);
        }
        public void TimerReset()
        {
            SetVisualsByState(State.StandBy);
            Reset();
        }

        private void EnableEntries(bool enable)
        {
            CooldownEntry.IsEnabled = enable;
            ActiveEntry.IsEnabled = enable;
            SetsEntry.IsEnabled = enable;
            StartUpEntry.IsEnabled = enable;
        }

        private void SetVisualsByState(State state)
        {
            switch (state)
            {
                case State.StandBy:
                    StatusText = "DONE";
                    StatusFrame.BackgroundColor = Color.AliceBlue;
                    break;
                case State.StartUp:
                    StatusText = "Get ready!";
                    StatusFrame.BackgroundColor = Color.CadetBlue;
                    break;
                case State.Active:
                    StatusText = "GO!";
                    StatusFrame.BackgroundColor = Color.LawnGreen;
                    break;
                case State.Cooldown:
                    StatusText = "Rest";
                    StatusFrame.BackgroundColor = Color.Yellow;
                    break;
                case State.Paused:
                    StatusText = "Paused";
                    StatusFrame.BackgroundColor = Color.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void Reset()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();

            _timer.Stop();
            _timer.Enabled = false;

            _startUp = true;
            TimerText = "0.0";
            EnableEntries(true);
        }
    }
}
