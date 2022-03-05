using Plugin.SimpleAudioPlayer;

namespace SuperSetTimer
{
    static class Audio
    {
        private static ISimpleAudioPlayer _player;
        public static void Load()
        {
            _player = CrossSimpleAudioPlayer.Current;
        }
        public static void PlayDone()
        {
            _player.Load("done.wav");
            _player.Play();
        }
        public static void PlayStartUp()
        {
            _player.Load("startup.wav");
            _player.Play();
        }
        public static void PlayActive()
        {
            _player.Load("active.ogg");
            _player.Play();
        }
        public static void PlayCooldown()
        {
            _player.Load("cooldown.ogg");
            _player.Play();
        }
        public static void PlayPause()
        {
            _player.Load("pause.ogg");
            _player.Play();
        }
    }
}
