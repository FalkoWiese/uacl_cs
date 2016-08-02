using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
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

            UaServer.RegisterObject(uaObject, parentObject);

            if (parentObject == null) return uaObject;

            var uaObjectList = (from pi in parentObject.GetType().GetProperties()
                let uaolAttribute = pi.GetCustomAttribute<UaObjectList>()
                where uaolAttribute != null
                select pi).FirstOrDefault();

            if (uaObjectList != null)
            {
                Type pot = parentObject.GetType();
                MethodInfo addUaNode = pot.GetMethod("AddUaNode");
                addUaNode.Invoke(parentObject, new[] { uaObjectList.GetValue(parentObject), uaObject });
                Logger.Info($"Added {uaObject} to {parentObject}.{uaObjectList.Name}");
            }
            else
            {
                Logger.Warn($"The parentObject for {uaObject} is not NULL, but cannot find a property on it what is annotated with <UaObjectList>!");
            }

            return uaObject;
        }

        private InternalServer UaServer { get; set; }
    }
}
