using System;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
	internal class InternalNodeManager : BaseNodeManager
	{
        private class SystemAddress
        {
            public int Address;
            public int Offset;
        }

        public InternalNodeManager(ServerManager server, params string[] namespaceUris)
			:base(server, namespaceUris)
		{
            CompanyUri = namespaceUris.Length < 1 || namespaceUris[0].Trim().Length <= 0 ?
                "http://www.company.com" : namespaceUris[0];

            ApplicationUri = namespaceUris.Length < 2 || namespaceUris[1].Trim().Length <= 0 ?
                "application" : namespaceUris[1];

            InstanceNamespaceIndex = AddNamespaceUri($"{CompanyUri}/{ApplicationUri}/instances");
            TypeNamespaceIndex = AddNamespaceUri($"{CompanyUri}/{ApplicationUri}/types");
        }

	    public string ApplicationUri { get; set; }
	    public string CompanyUri { get; set; }
        private ushort InstanceNamespaceIndex { get; set; }
        private ushort TypeNamespaceIndex { get; set; }

	    private InternalServerManager getManager()
	    {
	        return (InternalServerManager) this.Server;
	    }

        public override void Startup()
		{
			Console.WriteLine("InternalNodeManager: Startup()");

            CreateUaServerInterface();

            base.Startup();
		}

	    private void CreateUaServerInterface()
	    {
            // The root folder, named by the specific application, e.g. 'ServerConsole'.
	        var applicationRoot = CreateObjectTypeNode(new CreateObjectSettings()
	        {
	            ParentNodeId = ObjectIds.ObjectsFolder,
	            ReferenceTypeId = ReferenceTypeIds.Organizes,
	            RequestedNodeId = new NodeId(ApplicationUri, InstanceNamespaceIndex),
	            BrowseName = new QualifiedName(ApplicationUri, InstanceNamespaceIndex),
	            TypeDefinitionId = ObjectTypeIds.FolderType
	        });

            // Here we add the registered objects. Unless, they aren't correctly annotated.
	        foreach (var model in getManager().BusinessModel)
	        {
                var className = model.GetType().Name;
                CreateObjectTypeNode(new CreateObjectSettings()
                {
                    ParentNodeId = applicationRoot.NodeId,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    RequestedNodeId = new NodeId(className, InstanceNamespaceIndex),
                    BrowseName = new QualifiedName(className, InstanceNamespaceIndex),
                    TypeDefinitionId = ObjectTypeIds.BaseObjectType
                }, model);
            }
        }

        private ObjectNode CreateObjectTypeNode(CreateObjectSettings settings, object businessObject=null)
        {
            var node = CreateObject(Server.DefaultRequestContext, settings);
            Console.WriteLine($"Created node ... {node}.");

            if (businessObject != null)
            {
                       
            }

            return node;
        }

        public override void Shutdown()
		{
			base.Shutdown();
            Console.WriteLine(@"InternalNodeManager: Shutdown()");
        }
    }
}