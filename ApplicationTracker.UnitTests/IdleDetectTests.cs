using NSubstitute;
using System.Runtime.InteropServices;
using ApplicationTracker.Utilities;
using ApplicationTracker.Utilities.Interfaces;

namespace ApplicationTracker.UnitTests
{
    [TestFixture]
    public class IdleDetectTests
    {
        [Test]
        public void GetIdleTimeInfo_WhenCalled_ReturnsExpectedIdleTime()
        {
            // arrange
            var fakeHelper = Substitute.For<IIdleDetectHelper>();
            var idle = new IdleDetect();

            fakeHelper.GetSystemUptime().Returns(13);

            var systemUptime = 13;

            var lastInputInfo = new LASTINPUTINFO
            {
                cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO)),
                dwTime = 7
            };

            var lastInputTicks = (int)lastInputInfo.dwTime;

            fakeHelper
                .TryWrapperGetLastInputInfo(out Arg.Any<LASTINPUTINFO>())
                .Returns(x =>
                {
                    x[0] = lastInputInfo;
                    return true;
                });

            var idleTicks = systemUptime - lastInputTicks;

            // act
            var result = idle.GetIdleTimeInfo(fakeHelper);
            var expected = new IdleTimeInfo(idleTicks, systemUptime);

            // assert
            Assert.That(result.LastInputTime, Is.EqualTo(expected.LastInputTime).Within(TimeSpan.FromSeconds(1)));
            Assert.That(result.IdleTime, Is.EqualTo(expected.IdleTime));
            Assert.That(result.SystemUptimeMilliseconds, Is.EqualTo(expected.SystemUptimeMilliseconds));
        }
    }
}