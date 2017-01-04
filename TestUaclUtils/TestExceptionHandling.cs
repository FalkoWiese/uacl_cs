using System;
using System.Threading;
using NUnit.Framework;
using UaclUtils;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace TestUaclUtils
{
    [TestFixture]
    public class TestExceptionHandling
    {
        [Test]
        public void ExecuteReadProperty()
        {
            object o = new TestClass1(1);
            var rf = new ReflectionHelper(o);
            Assert.IsTrue(rf.ReadProperty<int>("Value1", "Value2", "Value") == 1);
            Assert.IsFalse(rf.ReadProperty<int>("Value1", "Value2", "Value") == 2);
            try
            {
                rf.ReadProperty<int>("Value1", "Value2");
                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.True(true, "Yup, here we expect an exception!");
            }
        }

        [Test]
        public void StopwatchHandling()
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"sw started ... {sw.Elapsed.TotalSeconds}");
            Thread.Sleep(1000);
            sw.Stop();
            var totalSeconds = sw.Elapsed.TotalSeconds;
            Assert.IsTrue(totalSeconds >= 1);
            Console.WriteLine($"sw stopped ... {totalSeconds}");
            sw.Start();
            Thread.Sleep(1000);
            sw.Stop();
            totalSeconds = sw.Elapsed.TotalSeconds;
            Assert.IsTrue(totalSeconds >= 2);
            Console.WriteLine($"sw second time stopped ... {totalSeconds}");
        }

        [Test]
        public void LogException()
        {
            var message = "An Exception.";
            try
            {
                try
                {
                    throw new Exception(message);
                }
                catch (Exception e1)
                {
                    Assert.True(e1 != null);
                    Assert.True(e1.Message.Equals(message));
                    ExceptionHandler.LogAndRaise(e1);
                }
            }
            catch (Exception e2)
            {
                Assert.True(e2 != null);
                Assert.True(e2.Message.Equals(message));
                ExceptionHandler.Log(e2);
                return;
            }
            Assert.Fail();
        }
    }
}