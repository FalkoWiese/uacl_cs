using System;
using System.Collections.Generic;
using System.Reflection;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
	public class InternalServerManager : ServerManager
	{
        public InternalServerManager(params string[] uris)
        {
            InternalUris = uris;
            BusinessModel = new List<object>();
        }

        private string[] InternalUris { get; set; }

		protected override void OnRootNodeManagerStarted(RootNodeManager nodeManager)
		{
			var mngr = new InternalNodeManager(this, InternalUris);
            mngr.Startup();
		}

        internal List<object> BusinessModel { get; set; }

	    public bool RegisterObject(object modelObject)
	    {
            try
            {
                var t = modelObject.GetType();
                var a = t.GetCustomAttribute<UaObject>();
                if (a == null)
                {
                    throw new Exception($"Cannot register UA object for type {t.Name}, it's not annotated with 'UaObject'!");
                }
            }
	        catch (Exception e)
	        {
                Console.WriteLine(e.Message);
                return false;
	        }

            BusinessModel?.Add(modelObject);
	        return true;
	    }
	}
}