using Plugin.SimpleAudioPlayer;
using Xamarin.Forms;

namespace SuperSetTimer
{
    class Audio
    {
        private readonly ISimpleAudioPlayer _player;

        public RadioButton MuteRadioButton { get; set; }
        public RadioButton EffectRadioButton { get; set; }

        private bool Mute => MuteRadioButton.IsChecked;
        private bool Effects => EffectRadioButton.IsChecked;

        public Audio()
        {
            _player = CrossSimpleAudioPlayer.Current;
        }

        public void PlayDone()
        {
            if(Mute)
                return;
            _player.Load(Effects ? "done.wav" : "game.wav");
            _player.Play();
        }
        public void PlayStartUp()
        {
            if (Mute)
                return;
            _player.Load(Effects ? "startup.wav" : "ready.wav");
            _player.Play();
        }
        public void PlayActive()
        {
            if (Mute)
                return;
            _player.Load(Effects ? "active.ogg" : "go.wav");
            _player.Play();
        }
        public void PlayCooldown()
        {
            if (Mute)
                return;
            _player.Load(Effects ? "cooldown.ogg" : "time.wav");
            _player.Play();
        }
        public void PlayPause()
        {
            if (Mute)
                return;
            _player.Load(Effects ? "pause.ogg" : "continue.wav");
            _player.Play();
        }
    }
}
