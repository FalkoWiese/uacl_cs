using System;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace UaclServer
{
	internal class InternalNodeManager : BaseNodeManager
	{
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

        public override void Startup()
		{
			Console.WriteLine("InternalNodeManager: Startup()");

            CreateInterface();

            base.Startup();
		}

	    private void CreateInterface()
	    {
            CreateObjectSettings settings = new CreateObjectSettings()
            {
                ParentNodeId = ObjectIds.ObjectsFolder,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                RequestedNodeId = new NodeId(ApplicationUri, InstanceNamespaceIndex),
                BrowseName = new QualifiedName(ApplicationUri, InstanceNamespaceIndex),
                TypeDefinitionId = ObjectTypeIds.FolderType
            };
            CreateObject(Server.DefaultRequestContext, settings);

	        var className = "BusinessLogic";
            settings = new CreateObjectSettings()
            {
                ParentNodeId = new NodeId(ApplicationUri, InstanceNamespaceIndex),
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                RequestedNodeId = new NodeId(className, InstanceNamespaceIndex),
                BrowseName = new QualifiedName(className, InstanceNamespaceIndex),
                TypeDefinitionId = ObjectTypeIds.BaseObjectType
            };
            CreateObject(Server.DefaultRequestContext, settings);
        }


        public override void Shutdown()
		{
			base.Shutdown();
            Console.WriteLine("InternalNodeManager: Shutdown()");
        }
    }
}