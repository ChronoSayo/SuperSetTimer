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
        private int _setsDone;
        private double _progressSpeed;
        private bool _isCooldown, _startUp;
        private State _state, _fromCountingState;

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
        public Button ActionButton { get; set; }
        public Button ResetButton { get; set; }

        private uint StartUpTime => uint.Parse(StartUpEntry.Text);
        private uint ActiveTime => uint.Parse(ActiveEntry.Text);
        private uint CooldownTime => uint.Parse(CooldownEntry.Text);
        private int Sets => int.Parse(SetsEntry.Text);

        private string TimerText
        {
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
        private string ActionButtonText
        {
            set => ActionButton.Text = value;
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

            _fromCountingState = State.Active;
            _state = State.StandBy;

            _stopWatch = new Stopwatch();
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                uint setTimer;
                if (_startUp)
                    setTimer = StartUpTime;
                else
                    setTimer = _isCooldown ? CooldownTime : ActiveTime;
                TimeSpan remainingTimer = TimeSpan.FromSeconds(setTimer) - _stopWatch.Elapsed;
                
                TimerText = remainingTimer.ToString(@"m\:ss\.ff");

                ProgressBar.Progress += _progressSpeed;
                
                if(remainingTimer.TotalMilliseconds > 0)
                    return;
                
                ProgressBar.Progress = 0;

                _isCooldown = !_isCooldown;
                
                if (_startUp) 
                    _startUp = false;

                if (!_isCooldown)
                {
                    _setsDone++;
                    SetText = "Sets: " + _setsDone + "/" + Sets;
                    SetState(State.Active);
                }
                else
                {
                    if (_setsDone >= Sets)
                    {
                        SetState(State.StandBy);
                        EnableEntries(true);
                        Reset();
                        return;
                    }

                    SetState(State.Cooldown);
                }

                _stopWatch.Restart();
            });
        }

        public void Action()
        {
            switch (_state)
            {
                case State.StandBy:
                    _stopWatch.Start();
                    _timer.Enabled = true;
                    _timer.Start();
                    _isCooldown = true;
                    _startUp = true;
                    _setsDone = 0;
                    ProgressBar.Progress = 0;
                    SetState(State.StartUp);
                    ResetButton.IsEnabled = false;
                    break;
                case State.Cooldown:
                case State.StartUp:
                case State.Active:
                    _stopWatch.Stop();
                    _timer.Stop();
                    _fromCountingState = _state;
                    SetState(State.Paused);
                    ResetButton.IsEnabled = true;
                    break;
                case State.Paused:
                    _stopWatch.Start();
                    _timer.Start();
                    ResetButton.IsEnabled = false;
                    SetState(_fromCountingState);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            EnableEntries(false);
        }
        public void TimerReset()
        {
            Reset();
        }

        private void EnableEntries(bool enable)
        {
            CooldownEntry.IsEnabled = enable;
            ActiveEntry.IsEnabled = enable;
            SetsEntry.IsEnabled = enable;
            StartUpEntry.IsEnabled = enable;
        }

        private void SetState(State state)
        {
            _state = state;
            Color bgColor = Color.AliceBlue;
            string statusText;
            switch (state)
            {
                case State.StandBy:
                    statusText = "DONE";
                    ActionButtonText = "START";
                    _fromCountingState = State.Active;
                    Audio.PlayDone();
                    break;
                case State.StartUp:
                    statusText = "Get ready!";
                    ActionButtonText = "PAUSE";
                    bgColor = Color.CadetBlue;
                    SetProgress(StartUpTime);
                    Audio.PlayStartUp();
                    break;
                case State.Active:
                    statusText = "GO!";
                    ActionButtonText = "PAUSE";
                    bgColor = Color.Green;
                    SetProgress(ActiveTime);
                    Audio.PlayActive();
                    break;
                case State.Cooldown:
                    statusText = "Rest";
                    ActionButtonText = "PAUSE";
                    bgColor = Color.Yellow;
                    SetProgress(CooldownTime);
                    Audio.PlayCooldown();
                    break;
                case State.Paused:
                    statusText = "Paused";
                    ActionButtonText = "RESUME";
                    bgColor = Color.Red;
                    Audio.PlayPause();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            StatusText = statusText;
            StatusFrame.BackgroundColor = bgColor;
        }

        private void SetProgress(uint time)
        {
            if(_fromCountingState == State.StandBy)
                return;

            _progressSpeed = 1.075 / (time * 1000);
        }

        private void Reset()
        {
            _stopWatch.Stop();
            _stopWatch.Reset();

            _timer.Stop();
            _timer.Enabled = false;

            _startUp = true;
            _setsDone = 0;
            ProgressBar.Progress = 0;

            TimerText = "0.0";
            StatusText = "-";
            SetText = "Sets: -/-";

            SetState(State.StandBy);
            EnableEntries(true);
        }
    }
}
