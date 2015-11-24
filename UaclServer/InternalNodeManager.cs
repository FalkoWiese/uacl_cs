using System;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
	internal class InternalNodeManager : BaseNodeManager
	{
		public InternalNodeManager(ServerManager server, params string[] namespaceUrises)
			:base(server, namespaceUrises)
		{
		    InternalNamespaceUris = namespaceUrises;
		}

        private string[] InternalNamespaceUris { get; set; }

		public override void Startup()
		{
			Console.WriteLine("InternalNodeManager: Startup()");
		    if (InternalNamespaceUris.Length > 0)
		    {
                AddNamespaceUri(InternalNamespaceUris[0]);
            }

            base.Startup();
		}


		public override void Shutdown()
		{
			base.Shutdown();
            Console.WriteLine("InternalNodeManager: Shutdown()");
        }
    }
}