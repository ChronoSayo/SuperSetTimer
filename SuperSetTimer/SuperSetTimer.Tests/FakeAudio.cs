using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SuperSetTimer.Tests
{
    class FakeAudio : IAudio
    {
        public RadioButton MuteRadioButton { get; set; }
        public RadioButton EffectRadioButton { get; set; }

        public void PlayDone()
        {
        }
        public void PlayPrepare()
        {
        }
        public void PlayActive()
        {
        }
        public void PlayCooldown()
        {
        }
        public void PlayPause()
        {
        }

        public void PlayCountdown(int countdown)
        {
        }
        public void ResetCountdown()
        {
        }
    }
}
