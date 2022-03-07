using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperSetTimer
{
    public class Countdown
    {
        private readonly Timer _timer;
        private readonly Stopwatch _stopWatch;
        private int _setsDone;
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
        public Label SetLabel { get; set; }
        public Frame StatusFrame { get; set; }
        public ProgressBar ProgressBar { get; set; }

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
        private string SetText
        {
            set => SetLabel.Text = value;
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
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            ProgressBar.Progress = 0;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                int setTimer;
                if (_startUp)
                    setTimer = StartUpTime;
                else
                    setTimer = _isCooldown ? CooldownTime : ActiveTime;
                TimeSpan remainingTimer = TimeSpan.FromSeconds(setTimer) - _stopWatch.Elapsed;
                
                TimerText = remainingTimer.ToString(@"m\:ss\.ff");

                if (remainingTimer.Seconds >= 0 && remainingTimer.Milliseconds >= 0)
                    return;

                _isCooldown = !_isCooldown;

                if (_startUp) 
                    _startUp = false;

                if (!_isCooldown)
                {
                    _setsDone++;
                    SetText = "Sets: " + _setsDone + "/" + Sets;
                    SetVisualsByState(State.Active);
                }
                else
                {
                    if (_setsDone >= Sets)
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
                _setsDone = 0;
                ProgressBar.Progress = 0;
                SetVisualsByState(State.StartUp);
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
            _paused = true;
            SetVisualsByState(State.Paused);
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

        private async void SetVisualsByState(State state)
        {
            ProgressBar.Progress = 0;
            Color bgColor = Color.AliceBlue;
            string statusText;
            int time = 0;
            switch (state)
            {
                case State.StandBy:
                    statusText = "DONE";
                    break;
                case State.StartUp:
                    statusText = "Get ready!";
                    bgColor = Color.CadetBlue;
                    time = StartUpTime;
                    break;
                case State.Active:
                    statusText = "GO!";
                    bgColor = Color.Green;
                    time = ActiveTime;
                    break;
                case State.Cooldown:
                    statusText = "Rest";
                    bgColor = Color.Yellow;
                    time = CooldownTime;
                    break;
                case State.Paused:
                    statusText = "Paused";
                    bgColor = Color.Red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            StatusText = statusText;
            StatusFrame.BackgroundColor = bgColor;
            
            await ShowProgressBar((uint)time);
        }

        private async Task ShowProgressBar(uint time)
        {
            ProgressBar.Progress = 0;
            await ProgressBar.ProgressTo(1, time * 1000, Easing.Linear);
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
