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

        public Audio()
        {
            _player = CrossSimpleAudioPlayer.Current;
        }

        public void SetSoundOptions(bool effects)
        {

        }

        public void PlayDone()
        {
            if(Mute)
                return;
            _player.Load("done.wav");
            _player.Play();
        }
        public void PlayStartUp()
        {
            if (Mute)
                return;
            _player.Load("startup.wav");
            _player.Play();
        }
        public void PlayActive()
        {
            if (Mute)
                return;
            _player.Load("active.ogg");
            _player.Play();
        }
        public void PlayCooldown()
        {
            if (Mute)
                return;
            _player.Load("cooldown.ogg");
            _player.Play();
        }
        public void PlayPause()
        {
            if (Mute)
                return;
            _player.Load("pause.ogg");
            _player.Play();
        }
    }
}
