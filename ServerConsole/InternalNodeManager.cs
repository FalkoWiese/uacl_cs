using UnifiedAutomation.UaServer;

namespace serverConsole
{
	internal class InternalNodeManager : BaseNodeManager
	{
		public InternalNodeManager(ServerManager server, params string[] namespaceUris)
			:base(server, namespaceUris)
		{
		}


		public override void Startup()
		{
			System.Console.WriteLine("InternalNodeManager: Startup");
			base.Startup();
		}


		public override void Shutdown()
		{
			System.Console.WriteLine("InternalNodeManager: Shutdown");
			base.Shutdown();
		}
	}
}