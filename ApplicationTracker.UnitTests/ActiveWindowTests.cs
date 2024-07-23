using System;
using NUnit;
using NSubstitute;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ApplicationTracker.Models;
using ApplicationTracker.Utilities;

namespace ApplicationTracker.UnitTests
{
    [TestFixture]
    public class ActiveWindowTests
    {
        [Test]
        public void IsActive_ActivatedHandleIsZero_ReturnsFalse()
        {
            // arrange
            var fakeHelper = Substitute.For<IActiveWindowHelper>();
            var activeWindow = new ActiveWindow(fakeHelper);
            fakeHelper.WrapperGetForegroundWindow().Returns(IntPtr.Zero);

            // act
            bool result = activeWindow.IsActive("test");

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsActive_ActiveProcEqualsArgProcName_ReturnsTrue()
        {
            // arrange
            var fakeHelper = Substitute.For<IActiveWindowHelper>();
            var activeWindow = new ActiveWindow(fakeHelper);
            fakeHelper.WrapperGetForegroundWindow().Returns(123);
            fakeHelper
                    .WrapperGetWindowThreadProcessId(123, out Arg.Any<int>())
                    .Returns(x =>
                    {
                        x[1] = 456;
                        return 1;
                    });

            fakeHelper.WrapperGetProcessById(456).Returns(new MyProcess { ProcessName = "test2" });

            // act
            var result = activeWindow.IsActive("test2");

            // assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsActive_ActiveProcDoesntEqualArgProcName_ReturnsFalse()
        {
            // arrange
            var fakeHelper = Substitute.For<IActiveWindowHelper>();
            var activeWindow = new ActiveWindow(fakeHelper);
            fakeHelper.WrapperGetForegroundWindow().Returns(123);

            fakeHelper
                    .WrapperGetWindowThreadProcessId(123, out Arg.Any<int>())
                    .Returns(x =>
                    {
                        x[1] = 456;
                        return 1;
                    });

            fakeHelper.WrapperGetProcessById(456).Returns(new MyProcess { ProcessName = "fakeName" });

            // act
            var result = activeWindow.IsActive("test2");

            // assert
            Assert.IsFalse(result);
        }
    }
}