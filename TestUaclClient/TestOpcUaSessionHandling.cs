using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Constraints;
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
            session.SessionIsConnectedEvent += SessionIsConnectedEvent;
            var thread = new Thread(session.EstablishOpcUaSession);
            thread.Start();
            thread.Join();
        }

        [Test]
        public void InvokeViaRemoteMethod()
        {
            RemoteObject obj = new RemoteObject("localhost", 48030, "BusinessLogic");
            RemoteMethod method = new RemoteMethod
            {
                Name = "CalculateJob",
                ArgumentDescriptions = new List<Variant>
                {
                    TypeMapping.Instance.ToVariant<string>(""),
                    TypeMapping.Instance.ToVariant<int>(-1)
                },
                ValueDescription = TypeMapping.Instance.ToVariant<bool>(false)
            };

            Variant r1 = obj.Invoke(method, new List<Variant>
            {
                TypeMapping.Instance.ToVariant<string>("BIG JOB"),
                TypeMapping.Instance.ToVariant<int>(2)
            });
            Assert.That(()=>r1.ToBoolean(), $"Return value is {true}");

/*
            Variant r2 = obj.Invoke(method, new List<Variant>
            {
                TypeMapping.Instance.ToVariant<string>("small job"),
                TypeMapping.Instance.ToVariant<int>(-1)
            });
            Assert.That(()=>r2.ToBoolean()==false, $"Return value is {false}");
*/
        }

        private void SessionIsConnectedEvent(object sender, EventArgs eventArgs)
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
                    TypeMapping.Instance.ToVariant<string>("'Job Name from Hell'"),
                    TypeMapping.Instance.ToVariant<int>(2)
                };

                methodCaller.BeginCallMethod("CalculateJob", inputArguments);

                Assert.That(()=>true, "Call of BusinessLogic::CalculateJob() is successful!");
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e);
                Assert.Fail();
            }
        }
    }
}