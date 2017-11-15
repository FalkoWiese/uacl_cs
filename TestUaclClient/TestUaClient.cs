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
            RemoteObject obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            Assert.That(() => obj.Connect(), "Is TRUE!");
            var r1 = obj.Invoke<bool>("CalculateJob", "BIG JOB", 2);
            Assert.That(() => r1, $"Return value is {true}");
            var r2 = obj.Invoke<bool>("CalculateJob", "small job", -1);
            Assert.That(() => !r2, $"Return value is {false}");
            var r3 = obj.Invoke<bool>("CalculateJob", "small job", -1);
            Assert.That(() => !r2, $"Return value is {false}");

            obj.Disconnect();
        }

        [Test]
        public void InvokeGetBytes()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            Assert.That(() => obj.Connect(), "Is TRUE!");
            const string value = "Moin from *UA Client* ...";
            var bytes = obj.Invoke<byte[]>("GetBytes", value);
            Assert.NotNull(bytes);

            obj.Disconnect();
        }

        [Test]
        public void WriteAndReadVariable()
        {
            var obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            Assert.That(() => obj.Connect(), "Is TRUE!");
            var s0 = obj.Read<string>("BoState");
            const string s1 = "THE NEW JOB STATE!";
            Assert.That(() => s0 != s1, $"'{s0}' != '{s1}'");
            obj.Write("BoState", s1);
            var s2 = obj.Read<string>("BoState");
            Assert.That(() => s1 == s2, $"'{s1}' == '{s2}'");
            obj.Write("BoState", s0);

            obj.Disconnect();
        }

        [Test]
        public void CallMethodSometimes()
        {
            var obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            Assert.That(() => obj.Connect(), "Is TRUE!");
            for (int i = 0; i < 50; i++)
            {
                var value = obj.Invoke<string>("JobStates");
                Assert.IsFalse(string.IsNullOrEmpty(value));
            }

            obj.Disconnect();
        }

        [Test]
        public void AddMonitoredItem()
        {
            var obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            Assert.That(() => obj.Connect(), "Is TRUE!");
            obj.Monitor(
                "BoState",
                v => { Logger.Info($"Received value from {obj.Name}.BoState ... '{v}'."); });

            obj.Disconnect();
        }

        [Test]
        public void InvokeJobStates()
        {
            var obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            Assert.That(() => obj.Connect(), "Is TRUE!");
            var value = obj.Invoke<string>("JobStates");
            Assert.IsFalse(string.IsNullOrEmpty(value));

            obj.Disconnect();
        }

        [Test]
        public void InvokeNullString()
        {
            var obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            Assert.That(() => obj.Connect(), "Is TRUE!");
            var value = obj.Invoke<string>("GetNullString");
            Assert.IsTrue(value == null);

            obj.Disconnect();
        }

//        [Test]
//        public void InvokeNullInteger()
//        {
//            var obj = new RemoteObject("localhost", 48030, "BusinessLogic");
//
//            Assert.That(() => obj.Connect(), "Is TRUE!");
//            var value = obj.Invoke<int?>("GetNullInteger");
//            Assert.IsTrue(value == null);
//
//            obj.Disconnect();
//        }

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

        [Test]
        public void AdvancedPathSplitting()
        {
            const string path = "PLC.Modules.<Default>.OPCua_Axis:opc_Axis.StatusX.Homing";

            string firstElement;
            var restOfPath = RemoteHelper.RestOfPath(path, out firstElement);
            Assert.IsTrue(firstElement == "PLC");
            Assert.IsTrue(restOfPath == "Modules.<Default>.OPCua_Axis:opc_Axis.StatusX.Homing");

            restOfPath = RemoteHelper.RestOfPath(restOfPath, out firstElement);
            Assert.IsTrue(firstElement == "Modules");
            Assert.IsTrue(restOfPath == "<Default>.OPCua_Axis:opc_Axis.StatusX.Homing");

            restOfPath = RemoteHelper.RestOfPath(restOfPath, out firstElement);
            Assert.IsTrue(firstElement == "<Default>");
            Assert.IsTrue(restOfPath == "OPCua_Axis:opc_Axis.StatusX.Homing");

            restOfPath = RemoteHelper.RestOfPath(restOfPath, out firstElement);
            Assert.IsTrue(firstElement == "OPCua_Axis");
            Assert.IsTrue(restOfPath == "opc_Axis.StatusX.Homing");

            restOfPath = RemoteHelper.RestOfPath(restOfPath, out firstElement);
            Assert.IsTrue(firstElement == "opc_Axis");
            Assert.IsTrue(restOfPath == "StatusX.Homing");

            restOfPath = RemoteHelper.RestOfPath(restOfPath, out firstElement);
            Assert.IsTrue(firstElement == "StatusX");
            Assert.IsTrue(restOfPath == "Homing");

            restOfPath = RemoteHelper.RestOfPath(restOfPath, out firstElement);
            Assert.IsTrue(firstElement == "Homing");
            Assert.IsTrue(restOfPath == "");
        }
    }
}