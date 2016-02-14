using System;
using System.Linq;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class RemoteObject
    {
        public RemoteObject(string ip, int port, string name)
        {
            Connection = new ConnectionInfo(ip, port);
            Name = name;
        }

        public ConnectionInfo Connection { get; }

        public string Name { get; }

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

                variable.Write(SessionHandler.Instance.GetSession(this).Session, this);
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

                var result = variable.Read(SessionHandler.Instance.GetSession(this).Session, this);
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
                OpcUaSession session = SessionHandler.Instance.GetSession(this).Session;
                Variant result = method.Invoke(session, this);
                return result;
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, $"Cannot invoke {Name}.{method.Name}() without errors!");
            }

            return method.HasReturnValue() ? method.ReturnValue : Variant.Null;
        }

    }
}
