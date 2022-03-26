using Xamarin.Forms;

namespace SuperSetTimer
{
    public interface IAudio
    {
        RadioButton MuteRadioButton { get; set; }
        RadioButton EffectRadioButton { get; set; }

        void PlayDone();
        void PlayStartUp();
        void PlayActive();
        void PlayCooldown();
        void PlayPause();
        void PlayCountdown(int countdown);
    }
}
