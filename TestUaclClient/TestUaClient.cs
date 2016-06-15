using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UaclClient;
using UaclUtils;

namespace TestUaclClient
{
    [TestFixture]
    public class TestUaClient
    {
        [Test]
        public void InvokeViaRemoteMethod()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "ServerConsole.BusinessLogic");

            var r1 = obj.Invoke<bool>("CalculateJob", (string) "BIG JOB", (int) 2);
            Assert.That(() => r1, $"Return value is {true}");

            var r2 = obj.Invoke<bool>("CalculateJob", (string) "small job", (int) -1);
            Assert.That(() => !r2, $"Return value is {false}");

            var r3 = obj.Invoke<bool>("CalculateJob", (string) "small job", (int) -1);
            Assert.That(() => !r2, $"Return value is {false}");
        }

        [Test]
        public void InvokeGetBytes()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "ServerConsole.BoProxy");
            string value = "Falko und Anke Wiese";
            var bytes = obj.Invoke<byte[]>("GetBytes", value);
            Assert.NotNull(bytes);
        }

        [Test]
        public void WriteAndReadVariable()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "ServerConsole.BusinessLogic");

            var s0 = obj.Read<string>("BoState");
            var s1 = "THE NEW JOB STATE!";
            Assert.That(() => s0 != s1, $"'{s0}' != '{s1}'");

            obj.Write("BoState", s1);
            var s2 = obj.Read<string>("BoState");
            Assert.That(() => s1 == s2, $"'{s1}' == '{s2}'");

            obj.Write("BoState", s0);
        }

        [Test]
        public void AddMonitoredItem()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "ServerConsole.BusinessLogic");
            obj.Monitor<string>("BoState", (string v)=> { Logger.Info($"Received value from {obj.Name}.BoState ... '{v}'.");});
        }

        [Test]
        public void BrowseServerTree()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "ServerConsole.BusinessLogic");
            var value = obj.Invoke<string>("JobStates");
            Assert.IsFalse(string.IsNullOrEmpty(value));
        }

        [Test]
        public void PathSplitting()
        {
            const string path = "ServerConsole.BusinessLogic.JobStates";

            string firstElement;
            var restOfPath = RemoteHelper.RestOfPath(path, out firstElement);
            Assert.IsTrue(firstElement == "ServerConsole");
            Assert.IsTrue(restOfPath == "BusinessLogic.JobStates");

            restOfPath = RemoteHelper.RestOfPath(restOfPath, out firstElement);
            Assert.IsTrue(firstElement == "BusinessLogic");
            Assert.IsTrue(restOfPath == "JobStates");

            restOfPath = RemoteHelper.RestOfPath(restOfPath, out firstElement);
            Assert.IsTrue(firstElement == "JobStates");
            Assert.IsTrue(restOfPath == "");
        }
    }
}