using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
	public class InternalServerManager : ServerManager
	{
        public InternalServerManager(string namespaceUri)
        {
            NamespaceUri = namespaceUri;
        }

        private string NamespaceUri { get; set; }

		protected override void OnRootNodeManagerStarted(RootNodeManager nodeManager)
		{
			var mngr = new InternalNodeManager(this, NamespaceUri);
            mngr.Startup();
		}
	}
}