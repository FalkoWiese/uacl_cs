using NUnit.Framework;
using ServerConsole;
using System.Threading;

namespace TestServerConsole
{
    [TestFixture]
    public class TestServerStart
    {
        [TestCase]
        public void ServerConsoleStart()
        {
            var server = new ServerConsoleServer();
            Assert.IsTrue(server.Start());
            Thread.Sleep(1000);
            Assert.IsTrue(server.Stop());
        }
    }
}
