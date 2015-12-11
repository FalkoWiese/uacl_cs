using NUnit.Framework;
using UaclServer;
using System.Threading;

namespace TestUaclServer
{
    [TestFixture]
    public class TestServerStart
    {
        [Test]
        public void StartAndStopFirstServer()
        {
            var server = new InternalServer("localhost", 48040, "FirstServer");
            Assert.IsTrue(server.Start());
            Thread.Sleep(1000);
            Assert.IsTrue(server.Stop());
        }

        [Test]
        public void RegisterAnObjectToSecondServer()
        {
            var server = new InternalServer("localhost", 48030, "SecondServer");
            Assert.IsNotNull(server);
            var bo = new BusinessObject();
            Assert.IsNotNull(bo);
            Assert.IsTrue(server.RegisterObject(bo));
        }


    }
}