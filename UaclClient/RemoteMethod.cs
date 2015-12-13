using System;
using System.Collections.Generic;
using System.Threading;
using UaclUtils;
using UnifiedAutomation.UaBase;

namespace UaclClient
{
    public class RemoteMethod
    {
        public string Name { get; set; }
        public Variant ValueDescription { get; set; }
        public List<Variant> ArgumentDescriptions { get; set; }

        public Variant Invoke(OpcUaSession session, List<Variant> inputArguments)
        {
            Variant methodValue = new Variant();
            session.SessionIsConnectedEvent += (object sender, EventArgs eventArgs) =>
            {
                try
                {
                    var opcUaSession = (OpcUaSession) sender;
                    var methodCaller = new MethodCaller(opcUaSession, "BusinessLogic",
                        (IAsyncResult result) =>
                        {
                            var callObj = (CallObjectsContainer)result.AsyncState;
                            try
                            {
                                // get the results.
                                List<StatusCode> inputArgumentErrors;
                                List<Variant> outputArguments;
                                // call the method.
                                var error = callObj.Session.EndCall(
                                    result,
                                    out inputArgumentErrors,
                                    out outputArguments);
                                // check for error.
                                if (StatusCode.IsBad(error))
                                {
                                    string message = $"Server returned an error while calling method: {error.ToString(true)}.\n";
                                    message += $"Method: {(callObj.Node != null ? callObj.Node.Identifier : "Method node object is null!")}";
                                    throw new Exception(message);
                                }
                                inputArgumentErrors.ForEach(x => Logger.Trace($"inputArgumentErrors from method call are: {x.ToString()}"));
                                outputArguments.ForEach(x => Logger.Trace($"outputArguments from method call are: {x}"));
                                if (outputArguments.Count > 1)
                                {
                                    throw new Exception("The number of output parameters is for now limited to ONE!");
                                }

                                methodValue = outputArguments[0];
                            }
                            catch (Exception e)
                            {
                                ExceptionHandler.Log(e, "An Exception occurred while finishing the asynchronous method call.");
                            }
                        });
                    foreach (var serverUri in opcUaSession.ServerUris)
                    {
                        Logger.Info(
                            $"Session Connectionstatus: {opcUaSession.ConnectionStatus}, Session Server URIS: {serverUri}");
                    }

                    methodCaller.BeginCallMethod(Name, inputArguments);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Log(e);
                }
            };

            var thread = new Thread(session.EstablishOpcUaSession);
            thread.Start();
            thread.Join();

            return methodValue;
        }
    }
}