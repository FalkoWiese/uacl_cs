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
        [Test]
        public void TestMethod1()
        {
            OpcUaSession session = OpcUaSession.Instance;
            session.SessionIsConnectedEvent += SessionIsConnectedEvent;
            var thread = new Thread(session.EstablishOpcUaSession);
            thread.Start();
            thread.Join();
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