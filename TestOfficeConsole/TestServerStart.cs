using System.Threading;
using NUnit.Framework;
using OfficeConsole;
using UaclServer;

namespace TestOfficeConsole
{
    [TestFixture]
    public class TestServerStart
    {
        [TestCase]
        public void OfficeConsoleServerStart()
        {
            InternalServer server = new OfficeConsoleServer();
            Assert.IsTrue(server.Start());
            Thread.Sleep(1000);
            Assert.IsTrue(server.Stop());
        }
    }
}
