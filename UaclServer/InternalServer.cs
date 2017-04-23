using System;
using System.Reflection;
using UaclUtils;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaSchema;
using ApplicationType = UnifiedAutomation.UaSchema.ApplicationType;

namespace UaclServer
{
    public class InternalServer
    {
        private int Port { get; set; }
        private string Ip { get; set; }
        private string ApplicationName { get; set; }

        private const string CompanyUri = "http://http://www.concept-laser.de";

        public InternalServer(string ip, int port, string applicationName)
        {
            Ip = ip;
            Port = port;
            ApplicationName = applicationName;
            Manager = new InternalServerManager(CompanyUri, ApplicationName);
        }

        // Fill in the application settings in code
        private void ConfigureOpcUaApplicationFromCode(ApplicationInstance instance, string ip, int port)
        {
            // The configuration settings are typically provided by another module
            // of the application or loaded from a data base. In this example the
            // settings are hardcoded
            var application = new SecuredApplication
            {
                // standard configuration options
                // general application identification settings
                // configure certificate stores
                // configure endpoints
                ApplicationName = "UnifiedAutomation GettingStartedServer",
                ApplicationUri = "urn:localhost:UnifiedAutomation:GettingStartedServer",
                ApplicationType = ApplicationType.Server_0,
                ProductName = "UnifiedAutomation GettingStartedServer",
                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%\unifiedautomation\UaSdkNet\pki\own",
                    SubjectName = "CN=GettingStartedServer/O=UnifiedAutomation/DC=localhost"
                },
                TrustedCertificateStore = new CertificateStoreIdentifier
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%\unifiedautomation\UaSdkNet\pki\trusted"
                },
                IssuerCertificateStore = new CertificateStoreIdentifier
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%\unifiedautomation\UaSdkNet\pki\issuers"
                },
                RejectedCertificatesStore = new CertificateStoreIdentifier
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%\unifiedautomation\UaSdkNet\pki\rejected"
                },
                BaseAddresses = new ListOfBaseAddresses {$"opc.tcp://{ip}:{port}"},
                SecurityProfiles = new ListOfSecurityProfiles
                {
                    new SecurityProfile {ProfileUri = SecurityProfiles.Basic256, Enabled = true},
                    new SecurityProfile {ProfileUri = SecurityProfiles.Basic128Rsa15, Enabled = true},
                    new SecurityProfile {ProfileUri = SecurityProfiles.None, Enabled = true}
                }
            };

            // Installation settings
            var installation = new InstallationSettings
            {
                GenerateCertificateIfNone = true,
                DeleteCertificateOnUninstall = true
            };
            application.Set(installation);

            // set the configuration for the application (must be called before start to have any effect).
            // these settings are discarded if the /configFile flag is specified on the command line.
            instance.SetApplicationSettings(application);
        }

        public bool Stop()
        {
            if (!Manager.IsRunning) return false;
            Manager.Stop();
            Logger.Info("UA Convenience Layer stopped.");
            return true;
        }

        private InternalServerManager Manager { get; set; }

        public bool RegisterObject(object modelObject, object parentObject = null)
        {
            return Manager?.RegisterObject(modelObject, parentObject) ?? false;
        }

        public void SetConnectCallback(Func<object, object, object> callback, object handlerContext)
        {
            Manager.SetConnectCallback(callback, handlerContext);
        }

        public void SetDisconnectCallback(Func<object, object, object> callback, object handlerContext)
        {
            Manager.SetDisconnectCallback(callback, handlerContext);
        }

        public bool Start()
        {
            if (Manager.IsRunning) return false;

            ApplicationLicenseManager.AddProcessLicenses(Assembly.GetExecutingAssembly(),
                "License.lic");
            var application = new ApplicationInstance {ConfigurationFilePath = "ServerConfig.xml"};
            application.LoadConfiguration(false, true);
            ConfigureOpcUaApplicationFromCode(application, Ip, Port);
            application.Start(Manager, o => { }, Manager);
            if (Manager.SessionManager != null)
            {
                Manager.SessionManager.SessionCreated += (session, reason) =>
                {
                    Logger.Info($"Client({session.Id.Identifier}) connected.");
                    if (Manager.ConnectHandler?.Callback == null) return;
                    var result = Manager.ConnectHandler.Callback(Manager.ConnectHandler.HandlerContext, session);
                    Manager.GetSessionContext()[session] = result;
                };
                Manager.SessionManager.SessionClosing += (session, reason) =>
                {
                    Logger.Info($"Client({session.Id.Identifier}) disconnected.");
                    if (Manager.DisconnectHandler?.Callback == null) return;
                    Manager.DisconnectHandler.Callback(Manager.DisconnectHandler.HandlerContext, session);
                    Manager.GetSessionContext().Remove(session);
                };
            }
            else
            {
                Logger.Warn(
                    "Cannot append events SessionCreated, and SessionClosing to the SessionManager, due to available SessionManager reference isn't null!");
            }

            Logger.Info("UA Convenience Layer is running ...");
            return true;
        }

        public void ReorganizeNodes()
        {
            Manager?.RegisterLaterOnAddedObjects();
        }
    }
}