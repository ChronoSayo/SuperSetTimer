using System;
using Xunit;
using SuperSetTimer;
using Xamarin.Forms;

namespace SuperSetTimer.Tests
{
    public class CountdownTests
    {
        [Fact]
        public void StartCountdown_Test()
        {
            Audio audio = new Audio()
            {
                EffectRadioButton = new RadioButton(),
                MuteRadioButton = new RadioButton()
            };
            Countdown countdown = new Countdown
            {
                StartUpEntry = new Entry(),
                ActiveEntry = new Entry(),
                CooldownEntry = new Entry(),
                SetsEntry = new Entry(),
                StatusLabel = new Label(),
                TimerLabel = new Label(),
                SetLabel = new Label(),
                StatusFrame = new Frame(),
                ProgressBar = new ProgressBar(),
                ActionButton = new Button(),
                ResetButton = new Button(),
                Audio = audio
            };

            countdown.Action();
            
            Assert.Equal("Get ready!", countdown.StatusLabel.Text);
        }
    }
}
