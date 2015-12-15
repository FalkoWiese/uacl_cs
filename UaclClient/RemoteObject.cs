using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// @Todo - Annotate RemoteMethod classes to execute a better type check for Name, InputParameter, and ReturnValue!
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        internal RemoteMethod Find(string methodName, List<Variant> arguments)
        {
            var method = Methods.FirstOrDefault(m => m.Name == methodName);

            if (method == null)
            {
                throw new Exception($"Cannot find method {Name}.{methodName}().");
            }

            if (method.InputArguments.Count != arguments.Count)
            {
                throw new Exception($"The number of arguments for {Name}.{method}() isn't equal to the description.");
            }

            for (var i = 0; i < arguments.Count; i++)
            {
                var desc = method.InputArguments[i];
                var arg = arguments[i];
                if (desc.DataType == arg.DataType) continue;
                throw new Exception($"The data types for argument number {i} of {Name}.{methodName}() aren't equal - description={desc.DataType}, given argument={arg.DataType}!");
            }

            return method;
        }

        public void Invoke(string name, params object[] parameters)
        {
            RemoteMethod method = new RemoteMethod
            {
                Name = name,
                InputArguments = parameters.Select(iA => TypeMapping.Instance.ToVariant(iA)).ToList(),
                ReturnValue = Variant.Null
            };
            // RegisterMethod(method); // @Todo - Registering should be something like a check for correct types etc.
            Invoke(method);
        }

        public T Invoke<T>(string name, params object[] parameters)
        {
            RemoteMethod method = new RemoteMethod
            {
                Name = name,
                InputArguments = parameters.Select(iA => TypeMapping.Instance.ToVariant(iA)).ToList(),
                ReturnValue = TypeMapping.Instance.MapType<T>()
            };
            // RegisterMethod(method); // @Todo - Registering should be something like a check for correct types etc.
            Variant returnValue = Invoke(method);
            return (T) TypeMapping.Instance.ToObject(returnValue);
        }

        public Variant Invoke(RemoteMethod method)
        {
            try
            {
                // RemoteMethod method = Find(remoteMethod.Name, remoteMethod.InputArguments); // @Todo - Replace the Find by a Check for the method signature.
                OpcUaSession session = SessionFactory.Instance.Create(Connection.Ip, Connection.Port).Session;
                Variant result = method.Invoke(session, this);
                return result;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogAndRaise(e, $"Cannot invoke {Name}.{method.Name}() without errors!");
            }

            return Variant.Null;
        }
    }
}
