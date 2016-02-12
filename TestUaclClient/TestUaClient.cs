using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UaclClient;
using UaclUtils;
using UnifiedAutomation.UaBase;

namespace TestUaclClient
{
    [TestFixture]
    public class TestUaClient
    {

        [Test]
        public void InvokeViaRemoteMethod()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            var r1 = obj.Invoke<bool>("CalculateJob", (string)"BIG JOB", (int)2);
            Assert.That(()=>r1, $"Return value is {true}");

            var r2 = obj.Invoke<bool>("CalculateJob", (string) "small job", (int) -1);
            Assert.That(()=>!r2, $"Return value is {false}");

            var r3 = obj.Invoke<bool>("CalculateJob", (string) "small job", (int) -1);
            Assert.That(()=>!r2, $"Return value is {false}");
        }

        [Test]
        public void InvokeGetBytes()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "BoProxy");
            string value = "Falko und Anke Wiese";
            var bytes = obj.Invoke<byte[]>("GetBytes", value);
            Assert.NotNull(bytes);
        }

        [Test]
        public void WriteAndReadVariable()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            var s1 = obj.Read<string>("State");
            var s2 = "THE NEW JOB STATE!";
            Assert.That(() => s1 != s2, $"'{s1}' != '{s2}'");

            obj.Write("State", s2);
            s1 = obj.Read<string>("State");
            Assert.That(() => s1 == s2, $"'{s1}' == '{s2}'");
        }
    }
}