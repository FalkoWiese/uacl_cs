using NUnit.Framework;
using UaclServer;
using System.Threading;
using ServerConsole;

namespace TestUaclServer
{
    [TestFixture]
    public class TestServerStart
    {
        [Test]
        public void StartAndStopTheServer()
        {
            var server = new InternalServer("localhost", 48040, "ControlConsole");
            Assert.IsTrue(server.Start());
            Thread.Sleep(1000);
            Assert.IsTrue(server.Stop());
        }

        [Test]
        public void RegisterAnObject()
        {
            var server = new InternalServer("localhost", 48030, "ServerConsole");
            Assert.IsNotNull(server);
            var bo = new BusinessLogic();
            Assert.IsNotNull(bo);
            Assert.IsTrue(server.RegisterObject(bo));
        }


    }
}