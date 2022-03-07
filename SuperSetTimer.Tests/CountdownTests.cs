using System;
using Xunit;
using SuperSetTimer;

namespace SuperSetTimer.Tests
{
    public class CountdownTests
    {
        [Fact]
        public void StartCountdown_Test()
        {
            Countdown countdown = new Countdown();
            countdown.Start();
            
            Assert.Equal("Get ready!", countdown.StatusLabel.Text);
        }
    }
}
