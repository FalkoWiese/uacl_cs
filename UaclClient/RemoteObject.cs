using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UaclClient;
using UaclUtils;
using UnifiedAutomation.UaBase;

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

        private readonly Lazy<List<RemoteMethod>> _lazyMethods = new Lazy<List<RemoteMethod>>(()=>new List<RemoteMethod>());
        private List<RemoteMethod> Methods => _lazyMethods.Value;

        public void RegisterMethod(RemoteMethod method)
        {
            RemoteMethod rm = Methods.FirstOrDefault(m => m.Name == method.Name);
            if (rm != null) return;
            Methods.Add(method);
        }

        internal RemoteMethod Find(string methodName, List<Variant> arguments)
        {
            RemoteMethod method = Methods.FirstOrDefault(m => m.Name == methodName);

            if (method == null)
            {
                throw new Exception($"Cannot find method {Name}.{methodName}().");
            };

            if (method.ArgumentDescriptions.Count != arguments.Count)
            {
                throw new Exception($"The number of arguments for {Name}.{method}() isn't equal to the description.");
            }

            for (var i = 0; i < arguments.Count; i++)
            {
                var desc = method.ArgumentDescriptions[i];
                var arg = arguments[i];
                if (desc.DataType == arg.DataType) continue;
                throw new Exception($"The data types for argument number {i} of {Name}.{methodName}() aren't equal - description={desc.DataType}, given argument={arg.DataType}!");
            }

            return method;
        }

        public Variant Invoke(RemoteMethod method, List<Variant> inputArguments)
        {
            RegisterMethod(method);
            return Invoke(method.Name, inputArguments);
        }

        public Variant Invoke(string methodName, List<Variant> inputArguments)
        {
            try
            {
                RemoteMethod method = Find(methodName, inputArguments);
                OpcUaSession session = SessionFactory.Instance.Create(Connection.Ip, Connection.Port).Session;
                Variant result = method.Invoke(session, inputArguments);
                return result;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogAndRaise(e, $"Cannot invoke {Name}.{methodName}() without errors!");
            }

            return Variant.Null;
        }
    }
}
