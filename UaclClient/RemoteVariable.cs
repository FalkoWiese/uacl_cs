using System;
using System.Collections.Generic;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class RemoteVariable
    {
        public string Name { get; set; }

        public Variant Value { get; set; }

        public Variant Read(OpcUaSession session, RemoteObject remoteObject)
        {
            return remoteObject.Execute(() =>
            {
                var vi = new VariableInvoker(session, remoteObject.Name);
                return vi.Read(this);
            }, session);
        }

        public void Write(OpcUaSession session, RemoteObject remoteObject)
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
                var vi = new VariableInvoker(session, remoteObject.Name);
                vi.Write(this);
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Error while invoking property '{Name}'.");
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