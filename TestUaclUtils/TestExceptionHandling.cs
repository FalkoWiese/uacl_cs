using System;
using NUnit.Framework;
using UaclUtils;

namespace TestUaclUtils
{
    [TestFixture]
    public class TestExceptionHandling
    {
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