using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class RemoteObject : IDisposable
    {
        public RemoteObject(string ip, int port, string name)
        {
            Connection = new ConnectionInfo(ip, port, name);
            Name = name;
            SessionLock = new object();
            SessionHandle = new OpcUaSessionHandle(OpcUaSession.Create(Connection));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool really)
        {
            if (really)
            {
                if (SessionHandle.Session.ConnectionStatus == ServerConnectionStatus.Connected)
                {
                    SessionHandle.Dispose();
                }
            }
        }

        public OpcUaSessionHandle SessionHandle { get; private set; }

        public ConnectionInfo Connection { get; }

        public string Name { get; }

        public void Monitor<T>(string name, Action<T> action)
        {
            try
            {
                var monitor = new RemoteDataMonitor<T>
                {
                    Name = name,
                    Value = TypeMapping.Instance.MapType<T>(),
                    Callback = action
                };

                monitor.Announce(this);
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Cannot subscribe MONITORED ITEM '{this.Name}.{name}'.");
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
            lock (SessionLock)
            {
                if (SessionHandle.Timeout)
                {
                    return Variant.Null;
                }

                var session = SessionHandle.Session;
                if (SessionHandle.Session.ConnectionStatus != ServerConnectionStatus.Connected)
                {
                    var stopWatch = Stopwatch.StartNew();

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
                        return Variant.Null;
                    }
                }

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
