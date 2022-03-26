﻿using System;
using Xamarin.Forms;

namespace SuperSetTimer
{
    public partial class MainPage : ContentPage
    {
        private readonly Countdown _countdown;

        public MainPage()
        {
            InitializeComponent();

            Audio audio = new Audio()
            {
                EffectRadioButton = EffectsRadioButton,
                MuteRadioButton = MuteRadioButton
            };
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
                SetLabel = SetLabel,
                ActionButton = ActionButton,
                ResetButton = ResetButton,
                Audio = audio
            };
        }

        private void OnTimerAction(object sender, EventArgs e)
        {
            _countdown.Action();
        }

        private void OnTimerReset(object sender, EventArgs e)
        {
            _countdown.TimerReset();
        }

        private void EntryInputCheck(object sender, EventArgs e)
        {
            if (_countdown.CorrectEntryNumber(((Entry) sender).Text, out string error)) 
                return;

            DisplayAlert("Wrong input", "Input can only be a number", "OK");
            ((Entry) sender).Text = error;
        }
    }
}
