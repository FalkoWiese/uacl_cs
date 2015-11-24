using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;
using System.Threading;

namespace UaclServer
{

    public class InternalServer
    { 
        private int Port { get; set; }
        private string Ip { get; set; }

        private Thread ServerThread { get; set; }

        public InternalServer(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
            ServerThread = new Thread(new ThreadStart(ServerMethod));
            Manager = new InternalServerManager("http://http://www.concept-laser.de/");
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
            catch (ThreadInterruptedException e)
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

            if (ServerThread.IsAlive)
            {
                ServerThread.Interrupt();
                ServerThread.Join();
                correctlyStopped = true && correctlyStopped;
            }

            return correctlyStopped;
        }

        private ServerManager Manager { get; set; }

        public bool Start()
        {
            if (ServerThread.IsAlive) return false;
            if (Manager.IsRunning) return false;

            ApplicationLicenseManager.AddProcessLicenses(System.Reflection.Assembly.GetExecutingAssembly(), "License.lic");
            var application = new ApplicationInstance();
            application.Start(Manager, RunServer, Manager);

            return true;
        }
    }
}
