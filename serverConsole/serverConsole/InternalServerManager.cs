using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace serverConsole
{
	public class InternalServerManager : ServerManager
	{
		protected override void OnRootNodeManagerStarted(RootNodeManager nodeManager)
		{
			new InternalNodeManager(this).Startup();



			/// [Add ImpersonateEventHandler]
			//this.SessionManager.ImpersonateUser += new ImpersonateEventHandler
			//(SessionManager_ImpersonateUser);
			// [Add ImpersonateEventHandler]
		}


		public override void Start(ApplicationInstance application)
		{
			base.Start(application);
		}


		public override void Stop()
		{
			base.Stop();
		}
	}
}