using Xunit;
using Xamarin.Forms;

namespace SuperSetTimer.Tests
{
    public class CountdownTests
    {
        private readonly Countdown _countdown;

        public CountdownTests()
        {
            Xamarin.Forms.Mocks.MockForms.Init();

            var audio = new FakeAudio()
            {
                EffectRadioButton = new RadioButton(),
                MuteRadioButton = new RadioButton()
            };
            _countdown = new Countdown
            {
                WorkoutsPicker = new Picker(),
                PrepareEntry = new Entry { Text = "1" },
                ActiveEntry1 = new Entry { Text = "1" },
                ActiveEntry2 = new Entry { Text = "1" },
                ActiveEntry3 = new Entry { Text = "1" },
                RestEntry = new Entry { Text = "1" },
                SetsEntry = new Entry { Text = "2" },
                StatusLabel = new Label { Text = "Status" },
                TimerLabel = new Label { Text = "1" },
                SetLabel = new Label { Text = "1" },
                WorkoutsLabel = new Label {Text = "3"},
                StatusFrame = new Frame(),
                ProgressBar = new ProgressBar(),
                ActionButton = new Button(),
                ResetButton = new Button(),
                Audio = audio
            };
            _countdown.AmountOfWorkouts(1);
        }

        [Fact]
        public void Countdown_Prepare_Test()
        {
            _countdown.Action();
            
            Assert.Equal("Get ready!", _countdown.StatusLabel.Text);
        }

        [Fact]
        public void Countdown_Active_Test()
        {
            _countdown.Action();
            
            while (_countdown.StatusLabel.Text != "GO!"){}

            Assert.Equal("GO!", _countdown.StatusLabel.Text);
        }

        [Fact]
        public void Countdown_Rest_Test()
        {
            _countdown.Action();

            while (_countdown.StatusLabel.Text != "Rest") { }

            Assert.Equal("Rest", _countdown.StatusLabel.Text);
        }

        [Fact]
        public void Countdown_Done_Test()
        {
            _countdown.Action();

            while (_countdown.StatusLabel.Text != "DONE") { }

            Assert.Equal("DONE", _countdown.StatusLabel.Text);
        }

        [Fact]
        public void Countdown_2WorkoutsDone_Test()
        {
            _countdown.AmountOfWorkouts(2);
            _countdown.Action();

            while (_countdown.StatusLabel.Text != "DONE") { }

            Assert.Equal("DONE", _countdown.StatusLabel.Text);
        }

        [Fact]
        public void Countdown_3WorkoutsDone_Test()
        {
            _countdown.AmountOfWorkouts(3);
            _countdown.Action();

            while (_countdown.StatusLabel.Text != "DONE") { }

            Assert.Equal("DONE", _countdown.StatusLabel.Text);
        }

        [Fact]
        public void Countdown_Paused_Test()
        {
            _countdown.Action();

            while (true)
            {
                if (_countdown.StatusLabel.Text != "GO!") continue;
                _countdown.Action();
                break;
            }

            Assert.Equal("Paused", _countdown.StatusLabel.Text);
        }

        [Fact]
        public void Countdown_UnPaused_Test()
        {
            _countdown.Action();

            while (true)
            {
                if (_countdown.StatusLabel.Text != "GO!") continue;
                _countdown.Action();
                _countdown.Action();
                break;
            }

            Assert.Equal("GO!", _countdown.StatusLabel.Text);
        }

        [Fact]
        public void Countdown_CorrectInput_Test()
        {
            bool correct = _countdown.CorrectEntryNumber("5", out string error);

            Assert.True(correct);
            Assert.Equal("1", error);
        }

        [Fact]
        public void Countdown_WrongInput_Test()
        {
            bool correct = _countdown.CorrectEntryNumber("a", out string error);

            Assert.False(correct);
            Assert.Equal("1", error);
        }

        [Fact]
        public void Countdown_Reset_Test()
        {
            _countdown.TimerReset();

            Assert.Equal("0", _countdown.TimerLabel.Text);
        }
    }
}
