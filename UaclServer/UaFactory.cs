using System;
using System.Linq;
using System.Reflection;
using UaclUtils;

namespace UaclServer
{
    internal class UaFactory
    {
        public UaFactory(InternalServer server)
        {
            UaServer = server;
        }

        public T CreateUaObject<T>(object parentObject=null)
        {
            var t = typeof (T);
            var c = t.GetConstructor(new Type[] {});
            var uaObject = (T) c?.Invoke(null);

            return CreateUaObject(uaObject, parentObject);
        }

        public T CreateUaObject<T>(T uaObject, object parentObject=null)
        {
            if (parentObject != null)
            {
                var uaObjectList = (from pi in parentObject.GetType().GetProperties()
                    let uaolAttribute = pi.GetCustomAttribute<UaObjectList>()
                    where uaolAttribute != null
                    select pi).FirstOrDefault();

                if (uaObjectList != null)
                {
                    var pot = parentObject.GetType();
                    var addUaNode = pot.GetMethod("AddUaNode");
                    if (addUaNode != null)
                    {
                        addUaNode.Invoke(parentObject, new[] {uaObjectList.GetValue(parentObject), uaObject});
                        Logger.Info($"Added {uaObject} to {parentObject}.{uaObjectList.Name}");
                    }
                    else
                    {
                        Logger.Warn(
                            $"Cannot add {uaObject} automatically to {parentObject}. Your business classes have to inherit from {typeof(ServerSideUaProxy).Name}!");
                    }
                }
                else
                {
                    Logger.Warn(
                        $"The parentObject for {uaObject} is not NULL, but cannot find a property on it what is annotated with <UaObjectList>!");
                }
            }
            else
            {
                if (!UaServer.RegisterObject(uaObject))
                {
                    Logger.Warn($"Cannot register {uaObject} to the UA Server.");
                }
            }

            return uaObject;
        }

        public T AddUaObject<T>(object parentObject = null)
        {
            var result = CreateUaObject<T>(parentObject);
            UaServer.ReorganizeNodes();
            return result;
        }

        private InternalServer UaServer { get; }
    }
}
