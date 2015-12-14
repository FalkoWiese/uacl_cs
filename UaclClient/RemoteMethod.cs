using System;
using System.Collections.Generic;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class RemoteMethod
    {
        public string Name { get; set; }
        public Variant ReturnValue { get; set; }
        public List<Variant> InputArguments { get; set; }

        public Variant Invoke(OpcUaSession session, RemoteObject remoteObject)
        {
            do
            {
                try
                {
                    Logger.Info($"Try to connect to:{session.SessionUri.Uri.AbsoluteUri}");
                    session.Connect(session.SessionUri.Uri.AbsoluteUri, SecuritySelection.None);
                    Logger.Info($"Connection to {session.SessionUri.Uri.AbsoluteUri} established.");
                }
                catch (Exception e)
                {
                    ExceptionHandler.Log(e,
                        $"An error occurred while try to connect to server: {session.SessionUri.Uri.AbsoluteUri}.");
                }
            } while (session.NotConnected());

            try
            {
                var methodCaller = new MethodCaller(session, remoteObject.Name);
                return methodCaller.CallMethod(this);
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Error while invoking method {Name}().");
                throw;
            }
            finally
            {
                if (!session.NotConnected())
                {
                    session.Disconnect();
                }
            }
        }
    }
}