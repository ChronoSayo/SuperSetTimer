using System;
using Xamarin.Forms;

namespace SuperSetTimer
{
    public partial class MainPage : ContentPage
    {
        private Countdown _countdown;
        public MainPage()
        {
            InitializeComponent();

            _countdown = new Countdown
            {
                StartUpEntry = StartUpEntry,
                ActiveEntry = ActiveEntry,
                CooldownEntry = CooldownEntry,
                SetsEntry = SetsEntry,
                StatusLabel = StatusLabel,
                TimerLabel = TimerLabel,
                StatusFrame = StatusFrame,
                ProgressBar = ProgressBar,
                SetLabel = SetLabel
            };
        }

        private void OnTimerStart(object sender, EventArgs e)
        {
            _countdown.Start();
        }

        private void OnTimerStop(object sender, EventArgs e)
        {
            _countdown.Stop();
        }

        private void OnTimerReset(object sender, EventArgs e)
        {
            _countdown.TimerReset();
        }
    }
}
