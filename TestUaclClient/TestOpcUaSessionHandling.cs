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
    public class TestOpcUaSessionHandling
    {
        //[Test]
        public void InvokeAMethodPlain()
        {
            OpcUaSession session = SessionFactory.Instance.Create("localhost", 48030).Session;
            session.SessionIsConnectedEvent += (sender, eventArgs) =>
            {
                try
                {
                    var opcUaSession = (OpcUaSession) sender;
                    var methodCaller = new MethodCaller(opcUaSession, "BusinessLogic");
                    foreach (var serverUri in opcUaSession.ServerUris)
                    {
                        Logger.Info(
                            $"Session Connectionstatus: {opcUaSession.ConnectionStatus}, Session Server URIS: {serverUri}");
                    }

                    var inputArguments = new List<Variant>
                    {
                        TypeMapping.Instance.ToVariant("'Job Name from Hell'"),
                        TypeMapping.Instance.ToVariant(2)
                    };

                    methodCaller.BeginCallMethod("CalculateJob", inputArguments);

                    Assert.That(()=>true, "Call of BusinessLogic::CalculateJob() is successful!");
                }
                catch (Exception e)
                {
                    ExceptionHandler.Log(e);
                    Assert.Fail();
                }
            };
            var thread = new Thread(session.EstablishOpcUaSession);
            thread.Start();
            thread.Join();
        }

        [Test]
        public void InvokeViaRemoteMethod()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "BusinessLogic");

            var r1 = obj.Invoke<bool>("CalculateJob", (string)"BIG JOB", (int)2);
            Assert.That(()=>r1, $"Return value is {true}");

            var r2 = obj.Invoke<bool>("CalculateJob", (string) "small job", (int) -1);
            Assert.That(()=>!r2, $"Return value is {false}");
        }
    }
}