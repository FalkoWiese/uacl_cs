using System;
using NUnit.Framework;
using UaclServer;
using System.Threading;

namespace TestUaclServer
{
    [TestFixture]
    public class TestServerStart
    {
        [Test]
        public void StartAndStopTheServer()
        {
            var server = new InternalServer("localhost", 48030);
            Assert.IsTrue(server.Start());
            Thread.Sleep(1000);
            server.Stop();
        }
    }
}