using System;
using System.Collections.Generic;
using System.Reflection;
using UnifiedAutomation.UaServer;
using UaclUtils;

namespace UaclServer
{
	public class InternalServerManager : ServerManager
	{
        private CallbackHandler ConnectHandler { get; set; }
        private CallbackHandler DisconnectHandler { get; set; }

	    internal class CallbackHandler
	    {
	        public Func<object, object, object> Callback { get; set; }
	        public object HandlerContext { get; set; }
	    } 

	    public void AnnounceConnectCallback(Func<object, object, object> callback, object handlerContext)
	    {
	        ConnectHandler = new CallbackHandler {Callback = callback, HandlerContext=handlerContext};
	    }

	    public void AnnounceDisconnectCallback(Func<object, object, object> callback, object handlerContext)
	    {
            DisconnectHandler = new CallbackHandler { Callback = callback, HandlerContext = handlerContext };
        }

	    private Dictionary<Session, object> _sessionContext;
        private Dictionary<Session, object> GetSessionContext()
        {
            return _sessionContext ?? (_sessionContext = new Dictionary<Session, object>());
        }

	    public void OnConnect(RequestContext clientContext)
	    {
	        object result = ConnectHandler.Callback(ConnectHandler.HandlerContext, clientContext);
	        Session clientSession = clientContext.Session;
	        GetSessionContext()[clientSession] = result;
	    }

	    public void OnDisconnect(RequestContext clientContext)
	    {
	        DisconnectHandler.Callback(DisconnectHandler.HandlerContext, clientContext);
	        Session clientSession = clientContext.Session;
	        GetSessionContext().Remove(clientSession);
	    }

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
                ExceptionHandler.Log(e);
                return false;
	        }

            BusinessModel?.Add(modelObject);
	        return true;
	    }
	}
}