using System;
using System.Diagnostics;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperSetTimer
{
    public class Countdown

    {
        private readonly Timer _timer;
        private readonly Stopwatch _stopWatch;
        private int _setsDone, _workouts, _currentWorkout;
        private uint _currentActiveTime;
        private double _progressSpeed;
        private bool _isCooldown, _startUp;
        private State _state, _fromCountingState;

        private enum State
        {
            StandBy, Prepare, PrepareNext, Active, Rest, Paused
        }
        
        public CheckBox RestOption { get; set; }
        public Entry PrepareNextEntry { get; set; }
        public Picker WorkoutsPicker { get; set; }
        public Entry PrepareEntry { get; set; }
        public Entry ActiveEntry1 { get; set; }
        public Entry ActiveEntry2 { get; set; }
        public Entry ActiveEntry3 { get; set; }
        public Entry RestEntry { get; set; }
        public Entry SetsEntry { get; set; }
        public Label TimerLabel { get; set; }
        public Label StatusLabel { get; set; }
        public Label WorkoutsLabel { get; set; }
        public Label SetLabel { get; set; }
        public Frame StatusFrame { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public Button ActionButton { get; set; }
        public Button ResetButton { get; set; }
        public IAudio Audio { get; set; }

        private uint StartUpTime => uint.Parse(PrepareEntry.Text);
        private uint PrepareNextTime => uint.Parse(PrepareNextEntry.Text);
        private uint ActiveTime1 => uint.Parse(ActiveEntry1.Text);
        private uint ActiveTime2 => uint.Parse(ActiveEntry2.Text);
        private uint ActiveTime3 => uint.Parse(ActiveEntry3.Text);
        private uint CooldownTime => uint.Parse(RestEntry.Text);
        private int Sets => int.Parse(SetsEntry.Text);

        private string TimerText
        {
            set => TimerLabel.Text = value;
        }
        private string StatusText
        {
            set => StatusLabel.Text = value;
        }
        private string WorkoutsText
        {
            set => WorkoutsLabel.Text = value;
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
            Device.BeginInvokeOnMainThread(() =>
            {
                bool noRest = !RestOption.IsChecked && _currentWorkout < _workouts;
                uint setTimer;
                if (_startUp)
                    setTimer = StartUpTime;
                else
                {
                    if (_isCooldown)
                    {
                        setTimer = CooldownTime;
                        if (noRest)
                            setTimer = PrepareNextTime;
                    }
                    else
                        setTimer = _currentActiveTime;
                }

                TimeSpan remainingTimer = TimeSpan.FromSeconds(setTimer) - _stopWatch.Elapsed;
                TimerText = remainingTimer.ToString(@"m\:ss\.ff");

                ProgressBar.Progress += _progressSpeed;

                Audio.PlayCountdown((int)remainingTimer.TotalSeconds + 1);
                
                if(remainingTimer.TotalMilliseconds > 0)
                    return;
                
                Audio.ResetCountdown();
                ProgressBar.Progress = 0;

                _isCooldown = !_isCooldown;
                
                if (_startUp) 
                    _startUp = false;

                if (!_isCooldown)
                    SetState(State.Active);
                else
                {
                    if (_setsDone == Sets && _currentWorkout == _workouts)
                    {
                        SetState(State.StandBy);
                        EnableEntries(true);
                        Reset();
                        UpdateWorkoutSetText();
                        return;
                    }

                    if(RestOption.IsChecked)
                        SetState(State.Rest);
                    else
                        SetState(noRest ? State.PrepareNext : State.Rest);
                }

                UpdateWorkoutSetText();
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
                    _currentWorkout = 0;
                    ProgressBar.Progress = 0;
                    SetState(State.Prepare);
                    ResetButton.IsEnabled = false;
                    SetScreenOn(true);
                    UpdateWorkoutSetText();
                    break;
                case State.Rest:
                case State.Prepare:
                case State.PrepareNext:
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
                    SetScreenOn(false);
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

        public void AmountOfWorkouts(int workouts)
        {
            _workouts = workouts;
            switch (_workouts)
            {
                case 1:
                    ActiveEntry2.IsEnabled = false;
                    ActiveEntry3.IsEnabled = false;
                    break;
                case 2:
                    ActiveEntry2.IsEnabled = true;
                    ActiveEntry3.IsEnabled = false;
                    break;
                case 3:
                    ActiveEntry2.IsEnabled = true;
                    ActiveEntry3.IsEnabled = true;
                    break;
            }

            UpdateWorkoutSetText();
        }

        public void EnablePrepareNextTime(bool enable)
        {
            PrepareNextEntry.IsEnabled = enable;
        }

        private void EnableEntries(bool enable)
        {
            WorkoutsPicker.IsEnabled = enable;
            RestOption.IsEnabled = enable;
            RestEntry.IsEnabled = enable;
            SetsEntry.IsEnabled = enable;
            PrepareEntry.IsEnabled = enable;
            ActiveEntriesEnable(enable);
            EnablePrepareNextTime(enable);
        }

        private void ActiveEntriesEnable(bool enable)
        {
            ActiveEntry1.IsEnabled = enable;
            if (_workouts == 2)
                ActiveEntry2.IsEnabled = enable;
            else if (_workouts > 2)
            {
                ActiveEntry2.IsEnabled = enable;
                ActiveEntry3.IsEnabled = enable;
            }
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
                case State.PrepareNext:
                case State.Prepare:
                    statusText = "Get ready!";
                    ActionButtonText = "PAUSE";
                    bgColor = Color.CadetBlue;
                    SetProgress(state == State.Prepare ? StartUpTime : PrepareNextTime);
                    Audio.PlayPrepare();
                    break;
                case State.Active:
                    statusText = "GO!";
                    ActionButtonText = "PAUSE";
                    bgColor = Color.Green;
                    if (_setsDone == 0)
                        _setsDone++;
                    _currentWorkout++;
                    if (_currentWorkout > _workouts)
                    {
                        _currentWorkout = 1;
                        _setsDone++;
                    }
                    switch (_currentWorkout)
                    {
                        case 1:
                            _currentActiveTime = ActiveTime1;
                            break;
                        case 2:
                            _currentActiveTime = ActiveTime2;
                            break;
                        case 3:
                            _currentActiveTime = ActiveTime3;
                            break;
                    }
                    SetProgress(_currentActiveTime);
                    Audio.PlayActive();
                    break;
                case State.Rest:
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

            ResetButton.IsEnabled = false;

            _startUp = true;
            _setsDone = 0;
            ProgressBar.Progress = 0;

            TimerText = "0";
            StatusText = "-";
            UpdateWorkoutSetText();

            SetScreenOn(false);

            SetState(State.StandBy);
            EnableEntries(true);
        }

        private void SetScreenOn(bool on)
        {
            if (DeviceInfo.DeviceType != DeviceType.Unknown)
                DeviceDisplay.KeepScreenOn = on;
        }

        private void UpdateWorkoutSetText()
        {
            WorkoutsText = "Workouts\n" + _currentWorkout + "/" + _workouts;
            SetText = "Sets\n" + _setsDone + "/" + Sets;
        }

        public bool CorrectEntryNumber(string text, out string error)
        {
            error = "1";
            return uint.TryParse(text, out _);
        }
    }
}
