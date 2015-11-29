using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;
using System.Threading;
using UnifiedAutomation.UaSchema;

namespace UaclServer
{

    public class InternalServer
    { 
        private int Port { get; set; }
        private string Ip { get; set; }
        private string ApplicationName { get; set; }
        private Thread ServerThread { get; set; }

        public const string CompanyUri = "http://http://www.concept-laser.de";

        public InternalServer(string ip, int port, string applicationName)
        {
            Ip = ip;
            Port = port;
            ApplicationName = applicationName;
            ServerThread = new Thread(new ThreadStart(ServerMethod));
            Manager = new InternalServerManager(CompanyUri, applicationName);
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
                ApplicationType = UnifiedAutomation.UaSchema.ApplicationType.Server_0,
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
                    new SecurityProfile() {ProfileUri = SecurityProfiles.Basic256, Enabled = true},
                    new SecurityProfile() {ProfileUri = SecurityProfiles.Basic128Rsa15, Enabled = true},
                    new SecurityProfile() {ProfileUri = SecurityProfiles.None, Enabled = true}
                }
            };
            // extended configuration options
            // trace settings
            var trace = new TraceSettings
            {
                MasterTraceEnabled = true,
                DefaultTraceLevel = UnifiedAutomation.UaSchema.TraceLevel.Info,
                TraceFile = @"%CommonApplicationData%\unifiedautomation\logs\ConfigurationServer.log.txt",
                MaxLogFileBackups = 3,
                ModuleSettings = new ModuleTraceSettings[]
                {
                    new ModuleTraceSettings() {ModuleName = "UnifiedAutomation.Stack", TraceEnabled = true},
                    new ModuleTraceSettings() {ModuleName = "UnifiedAutomation.Server", TraceEnabled = true},
                }
            };
            application.Set<TraceSettings>(trace);
            // Installation settings
            var installation = new InstallationSettings
            {
                GenerateCertificateIfNone = true,
                DeleteCertificateOnUninstall = true
            };
            application.Set<InstallationSettings>(installation);
            // set the configuration for the application (must be called before start to have any effect).
            // these settings are discarded if the /configFile flag is specified on the command line.
            instance.SetApplicationSettings(application);
        }


        private void RunServer(object state)
        {
            ServerThread.Start();
        }

        private void ServerMethod()
        {
            Console.WriteLine("UA Convenience Layer is running ...");
            try
            {
                while (ServerThread.IsAlive)
                {
                    Thread.Sleep(1);
                }
            }
            catch (ThreadInterruptedException)
            {
                Console.WriteLine("UA Convenience Layer stopped.");
            }
        }

        public bool Stop()
        {
            var correctlyStopped = false;

            if (Manager.IsRunning)
            {
                Manager.Stop();
                correctlyStopped = true;
            }

            if (!ServerThread.IsAlive) return correctlyStopped;

            ServerThread.Interrupt();
            ServerThread.Join();
            correctlyStopped = true && correctlyStopped;

            return correctlyStopped;
        }

        private InternalServerManager Manager { get; set; }

        public bool RegisterObject(object modelObject)
        {
            return Manager != null && Manager.RegisterObject(modelObject);
        }


        public bool Start()
        {
            if (ServerThread.IsAlive) return false;
            if (Manager.IsRunning) return false;

            ApplicationLicenseManager.AddProcessLicenses(System.Reflection.Assembly.GetExecutingAssembly(), "License.lic");
            var application = new ApplicationInstance();
            ConfigureOpcUaApplicationFromCode(application, Ip, Port);
            application.Start(Manager, RunServer, Manager);

            return true;
        }
    }
}
