using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
	public class InternalServerManager : ServerManager
	{
        public InternalServerManager(params string[] uris)
        {
            InternalUris = uris;
        }

        private string[] InternalUris { get; set; }

		protected override void OnRootNodeManagerStarted(RootNodeManager nodeManager)
		{
			var mngr = new InternalNodeManager(this, InternalUris);
            mngr.Startup();
		}
	}
}