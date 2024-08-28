using ApplicationTracker.Models;
using ApplicationTracker.Utilities;
using NSubstitute;
using System.Collections.ObjectModel;


namespace ApplicationTracker.UnitTests
{
    [TestFixture]
    public class ProcHelperTests
    {
        [Test]
        public void UpdateRunningProcs_ProcInRunningProcsIsActive_IncrementProcTime()
        {
            // arrange
            //var fakeActiveWindowHelper = Substitute.For<IActiveWindowHelper>();
            var fakeActiveWindow = Substitute.ForPartsOf<ActiveWindow>();

            fakeActiveWindow.IsActive(Arg.Is("test")).Returns(true);

            ObservableCollection<ProcessWrapper> runningProcs = new ObservableCollection<ProcessWrapper>();

            runningProcs.Add(
                new ProcessWrapper()
                {
                    ProcessName = "test",
                    ProcessTime = TimeSpan.FromSeconds(10),
                    ProcessDate = new DateTime(2000, 1, 1, 0, 0, 0)
                });

            runningProcs.Add(
                new ProcessWrapper()
                {
                    ProcessName = "test2",
                    ProcessTime = TimeSpan.FromSeconds(15),
                    ProcessDate = new DateTime(2000, 1, 1, 0, 0, 0)
                });

            ProcHelper procHelper = new ProcHelper();

            // act
            procHelper.UpdateRunningProcs(runningProcs, fakeActiveWindow);
            //var result = runningProcs[0]._processtime;

            // assert
            Assert.That(runningProcs[0]._processtime, Is.EqualTo(TimeSpan.FromSeconds(11)));
            Assert.That(runningProcs[1]._processtime, Is.EqualTo(TimeSpan.FromSeconds(15)));
        }

        [Test]
        public void UpdateRunningProcs_ProcInRunningProcsIsntActive_ProcTimeUnchanged()
        {
            // arrange
            //var fakeActiveWindowHelper = Substitute.For<IActiveWindowHelper>();
            var fakeActiveWindow = Substitute.ForPartsOf<ActiveWindow>();

            fakeActiveWindow.IsActive(Arg.Any<string>()).Returns(false);

            ObservableCollection<ProcessWrapper> runningProcs = new ObservableCollection<ProcessWrapper>();

            runningProcs.Add(
                new ProcessWrapper()
                {
                    ProcessName = "test",
                    ProcessTime = TimeSpan.FromSeconds(10),
                    ProcessDate = new DateTime(2000, 1, 1, 0, 0, 0)
                });

            runningProcs.Add(
                new ProcessWrapper()
                {
                    ProcessName = "test",
                    ProcessTime = TimeSpan.FromSeconds(15),
                    ProcessDate = new DateTime(2000, 1, 1, 0, 0, 0)
                });

            ProcHelper procHelper = new ProcHelper();

            // act
            procHelper.UpdateRunningProcs(runningProcs, fakeActiveWindow);
            var firstProc = runningProcs[0]._processtime;
            var secondProc = runningProcs[1]._processtime;

            // assert
            Assert.That(firstProc, Is.EqualTo(TimeSpan.FromSeconds(10)));
            Assert.That(secondProc, Is.EqualTo(TimeSpan.FromSeconds(15)));
        }

        [Test]
        public void ProcTimer_NotIdle_IncrementActiveProcTime()
        {
            // arrange
            var fakeProcHelper = Substitute.ForPartsOf<ProcHelper>();
            var fakeIdleDetect = Substitute.ForPartsOf<IdleDetect>();
            var fakeActiveWindow = Substitute.ForPartsOf<ActiveWindow>();

            fakeProcHelper.When(x => x.GetRunningProcs(Arg.Any<HashSet<string>>(), Arg.Any<ObservableCollection<ProcessWrapper>>())).DoNotCallBase();
            fakeIdleDetect.GetIdleTimeInfo(Arg.Any<IIdleDetectHelper>()).Returns(new IdleTimeInfo(0, 0));

            fakeActiveWindow.IsActive(Arg.Is("test")).Returns(true);

            HashSet<string> exclusionList = new HashSet<string>
            {
                //"explorer",
                "textinputhost",
                "ApplicationFrameHost",
                "svchost",
                //"devenv",
                "TextInputHost",
                "updatechecker",
            };

            ObservableCollection<ProcessWrapper> runningProcs = new ObservableCollection<ProcessWrapper>();

            runningProcs.Add(
                new ProcessWrapper()
                {
                    ProcessName = "test",
                    ProcessTime = TimeSpan.FromSeconds(10),
                    ProcessDate = new DateTime(2000, 1, 1, 0, 0, 0)
                });

            runningProcs.Add(
                new ProcessWrapper()
                {
                    ProcessName = "test2",
                    ProcessTime = TimeSpan.FromSeconds(15),
                    ProcessDate = new DateTime(2000, 5, 5, 0, 0, 5)
                });

            // act
            fakeProcHelper.ProcTimer(exclusionList, runningProcs, fakeIdleDetect, fakeActiveWindow);
            var firstProc = runningProcs[0]._processtime;
            var secondProc = runningProcs[1]._processtime;

            // assert
            Assert.That(firstProc, Is.EqualTo(TimeSpan.FromSeconds(11)));
            Assert.That(secondProc, Is.EqualTo(TimeSpan.FromSeconds(15)));
        }
        
    }

}

