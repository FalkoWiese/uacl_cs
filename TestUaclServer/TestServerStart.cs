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
            server.Stop();
        }

        [Test]
        public void RegisterAnObject()
        {
            var server = new InternalServer("localhost", 48030, "ServerConsole");

            var bo = new BusinessLogic();
            server.RegisterObject(bo);

            Assert.IsTrue(server.Start());
        }
    }
}