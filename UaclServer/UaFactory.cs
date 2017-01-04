using System;
using System.Linq;
using System.Reflection;
using UaclUtils;

namespace UaclServer
{
    public class UaFactory
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

            PropertyInfo uaObjectList = null;
            if (parentObject != null)
            {
                uaObjectList = (from pi in parentObject.GetType().GetProperties()
                                let uaolAttribute = pi.GetCustomAttribute<UaObjectList>()
                                where uaolAttribute != null
                                select pi).FirstOrDefault();
            }

            if (uaObjectList != null)
            {
                var pot = parentObject.GetType();
                var addUaNode = pot.GetMethod("AddUaNode");
                if (addUaNode != null)
                {
                    addUaNode.Invoke(parentObject, new[] { uaObjectList.GetValue(parentObject), uaObject });
                    Logger.Info($"Added {uaObject} to {parentObject}.{uaObjectList.Name}");
                }
                else
                {
                    Logger.Warn($"Cannot add {uaObject} automatically to {parentObject}. Your business classes have to inherit from {typeof(ServerSideUaProxy).Name}!");
                }
            }
            else
            {
                UaServer.RegisterObject(uaObject);
                Logger.Warn($"The parentObject for {uaObject} is not NULL, but cannot find a property on it what is annotated with <UaObjectList>!");
            }

            return uaObject;
        }

        private InternalServer UaServer { get; }
    }
}
