using System.Collections.Generic;
using Plugin.SimpleAudioPlayer;
using Xamarin.Forms;

namespace SuperSetTimer
{
    public class Audio : IAudio
    {
        private readonly ISimpleAudioPlayer _player;
        private readonly List<bool> _countdowns;

        public RadioButton MuteRadioButton { get; set; }
        public RadioButton EffectRadioButton { get; set; }
        
        bool Mute => MuteRadioButton.IsChecked;
        bool Effects => EffectRadioButton.IsChecked;

        public Audio()
        {
            _player = CrossSimpleAudioPlayer.Current;
            _player.Loop = false;
            _countdowns = new List<bool>();
            for (int i = 0; i < 5; i++)
                _countdowns.Add(false);
        }

        public void PlayDone()
        {
            if(Mute)
                return;
            _player.Load(Effects ? "done.wav" : "game.wav");
            _player.Play();
        }
        public void PlayPrepare()
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

        public void PlayCountdown(int countdown)
        {
            if(Mute || _player.IsPlaying)
                return;

            int i = countdown - 1;
            switch (countdown)
            {
                case 5:
                case 4:
                case 3:
                case 2:
                case 1:
                    if(!_countdowns[i] || Mute)
                        _countdowns[i] = LoadAndPlayCountdown(Effects ? "countdown.ogg" : countdown + ".wav", _countdowns[i]);
                    break;
            }
        }

        public void ResetCountdown()
        {
            for (int i = 0; i < _countdowns.Count; i++)
                _countdowns[i] = false;
        }

        private bool LoadAndPlayCountdown(string filename, bool played)
        {
            _player.Load(filename);
            _player.Play();

            return true;

        }
    }
}
