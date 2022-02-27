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
        private Countdown _countdown;
        public MainPage()
        {
            _countdown = new Countdown
            {
                StartUpEntry = StartUpEntry,
                ActiveEntry = ActiveEntry,
                CooldownEntry = CooldownEntry,
                SetsEntry = SetsEntry,
                StatusLabel = StatusLabel,
                TimerLabel = TimerLabel
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
