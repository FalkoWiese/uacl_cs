using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class RemoteObject
    {
        public RemoteObject(string ip, int port, string name)
        {
            Connection = new ConnectionInfo(ip, port, name);
            Name = name;
            MyNodeId = NodeId.Null;
            NodeIdCache = new Dictionary<string, NodeId>();
            SessionLock = new object();
            ConnectionEstablishmentLock = new object();
            StartConnectionEstablishmentCallback = () =>
            {
                if (ConnectionEstablishmentIsWorking) return;

                try
                {
                    lock (ConnectionEstablishmentLock)
                    {
                        ConnectionEstablishmentIsWorking = true;
                    }

                    while (true)
                    {
                        try
                        {
                            lock (SessionLock)
                            {
                                if (SessionHandle == null)
                                {
                                    SessionHandle = new OpcUaSessionHandle(OpcUaSession.Create(Connection));
                                }
                            }
                        }
                        catch (Exception exc)
                        {
                            Logger.Warn(
                                $"Exception at connection establishment while 'new OpcUaSessionHandle()' ... '{exc.Message}'.");
                            Thread
                                .Sleep(1000); // It's maybe a good idea, not to have a hectic connection establishment.
                            continue;
                        }

                        try
                        {
                            if (Connect())
                            {
                                return;
                            }
                        }
                        catch (Exception exc)
                        {
                            Logger.Warn(
                                $"Exception at connection establishment while 'Connect()' ... '{exc.Message}'.");
                            Thread
                                .Sleep(1000); // It's maybe a good idea, not to have a hectic connection establishment.
                        }
                    }
                }
                finally
                {
                    lock (ConnectionEstablishmentLock)
                    {
                        ConnectionEstablishmentIsWorking = false;
                    }
                }
            };

            StartConnectionEstablishment(); // Yup, we'll call it while RemoteObject creation, directly.
        }

        private object ConnectionEstablishmentLock { get; set; }
        private bool ConnectionEstablishmentIsWorking { get; set; }
        private ThreadStart StartConnectionEstablishmentCallback { get; set; }

        public void StartConnectionEstablishment()
        {
            var thread = new Thread(StartConnectionEstablishmentCallback);

            thread.Start();
        }

        private event Action<Session, ServerConnectionStatusUpdateEventArgs> NotConnectedCallback;

        private Action PostConnectionEstablished { get; set; }

        public NodeId MyNodeId { get; set; }
        public Dictionary<string, NodeId> NodeIdCache { get; set; }

        public void SetDisconnectedHandler(Action<Session, ServerConnectionStatusUpdateEventArgs> handler)
        {
            AnnounceSessionNotConnectedHandler(handler);
        }

        protected void AnnounceSessionNotConnectedHandler(
            Action<Session, ServerConnectionStatusUpdateEventArgs> notConnected)
        {
            NotConnectedCallback += notConnected;
        }

        protected void AnnouncePostConnectionEstablishedHandler(Action postConnectionEstablished)
        {
            PostConnectionEstablished = postConnectionEstablished;
        }

        private bool AnnounceToSession()
        {
            ServerConnectionStatusUpdateEventHandler statusChangedCallback = (s, args) =>
            {
                switch (s.ConnectionStatus)
                {
                    case ServerConnectionStatus.ConnectionErrorClientReconnect:
                    case ServerConnectionStatus.Disconnected:
                    case ServerConnectionStatus.LicenseExpired:
                    case ServerConnectionStatus.ServerShutdown:
                    case ServerConnectionStatus.ServerShutdownInProgress:
                        NotConnectedCallback?.Invoke(s, args);
                        // My idea was, to call Dispose() here, but I think, we should do it from
                        // outside the RemoteObject context ...
                        return;
                    case ServerConnectionStatus.Connected:
                    case ServerConnectionStatus.SessionAutomaticallyRecreated:
                        PostConnectionEstablished?.Invoke();
                        // I think, it's a good idea, to have a callback like this. So you can provide e. g. the
                        // monitoring of some UA variables here.
                        return;
                    default:
                        /*
                            case ServerConnectionStatus.Connecting:
                            case ServerConnectionStatus.ConnectionWarningWatchdogTimeout:
                        */
                        return;
                }
            };

            return SessionHandle.AddStatusChangedHandler(statusChangedCallback);
        }

        public bool Connected()
        {
            lock (SessionLock)
            {
                return SessionHandle != null && SessionHandle.Session.ConnectionStatus ==
                       ServerConnectionStatus.Connected;
            }
        }

        public bool Connect()
        {
            if (Connected())
            {
                return true;
            }

            lock (SessionLock)
            {
                if (SessionHandle == null || SessionHandle.Timeout)
                {
                    return false;
                }

                var session = SessionHandle.Session;
                var stopWatch = Stopwatch.StartNew();

                if (AnnounceToSession())
                {
                    Logger.Info("ConnectionStatusChanged callback successfull registered!");
                }

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

                    stopWatch.Stop();
                    if (stopWatch.Elapsed.Seconds > 5)
                    {
                        break;
                    }

                    stopWatch.Start();
                } while (session.NotConnected());

                if (session.NotConnected())
                {
                    SessionHandle.Timeout = true;
                    return false;
                }
            }

            return Connected();
        }

        public void Disconnect()
        {
            if (!Connected()) return;

            lock (SessionLock)
            {
                SessionHandle?.Dispose();
            }
        }

        public OpcUaSessionHandle SessionHandle { get; private set; }

        public ConnectionInfo Connection { get; }

        public string Name { get; }

        public void Monitor(Dictionary<string, Action<Variant>> monitors)
        {
            Execute(() =>
            {
                var rh = new RemoteHelper(this);
                rh.MonitorDataChanges(monitors.Keys.Select(name => new RemoteDataMonitor
                    {
                        Name = name,
                        Value = Variant.Null,
                        Callback = monitors[name]
                    })
                    .ToList(), this);
                return Variant.Null;
            });
        }

        public void Monitor(string name, Action<Variant> action)
        {
            try
            {
                var monitor = new RemoteDataMonitor
                {
                    Name = name,
                    Value = Variant.Null,
                    Callback = action
                };

                monitor.Announce(this);
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Cannot subscribe MONITORED ITEM '{Name}.{name}'.");
            }
        }

        protected void Invoke(string name, params object[] parameters)
        {
            var method = new RemoteMethod
            {
                Name = name,
                InputArguments = parameters.Select(iA => TypeMapping.Instance.ToVariant(iA)).ToList(),
                ReturnValue = Variant.Null
            };

            Invoke(method);
        }

        public T Invoke<T>(string name, params object[] parameters)
        {
            var method = new RemoteMethod
            {
                Name = name,
                InputArguments = parameters.Select(iA => TypeMapping.Instance.ToVariant(iA)).ToList(),
                ReturnValue = TypeMapping.Instance.MapType<T>()
            };

            var returnValue = Invoke(method);
            return (T) TypeMapping.Instance.ToObject(returnValue);
        }

        public void Write(string name, object parameter)
        {
            try
            {
                var variable = new RemoteVariable
                {
                    Name = name,
                    Value = TypeMapping.Instance.ToVariant(parameter)
                };

                variable.Write(this);
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Cannot write {Name}.{name} without errors!");
            }
        }

        public T Read<T>(string name)
        {
            try
            {
                var variable = new RemoteVariable
                {
                    Name = name,
                    Value = TypeMapping.Instance.MapType<T>()
                };

                var result = variable.Read(this);
                return (T) TypeMapping.Instance.ToObject(result);
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Cannot read {Name}.{name} without errors!");
            }

            return (T) TypeMapping.Instance.ToObject(TypeMapping.Instance.MapType<T>());
        }

        private Variant Invoke(RemoteMethod method)
        {
            try
            {
                Variant result = method.Invoke(this);
                return result;
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Cannot invoke {Name}.{method.Name}() without errors!");
            }

            return method.HasReturnValue() ? method.ReturnValue : Variant.Null;
        }

        private object SessionLock { get; set; }

        public Variant Execute(Func<Variant> action)
        {
            if (!Connected())
            {
                throw new Exception("Cannot execute given client action, due to an unavailable connection!");
            }

            lock (SessionLock)
            {
                try
                {
                    return action();
                }
                catch (Exception e)
                {
                    ExceptionHandler.Log(e, $"Error while invoke something on '{Name}'.");
                    throw;
                }
            }
        }
    }
}