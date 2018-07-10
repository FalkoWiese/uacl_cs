using System;
using System.Linq;
using System.Threading;
using UaclClient;
using UaclServer;
using UaclUtils;

namespace MultiClientConsole
{
    class Program
    {
        static void Main()
        {
            InternalServer server = null;
            try
            {
                server = new MultiClientServer();
                // The Multi Client Host ...
                var parent = server.CreateClient<MultiClientHost>();
                // The Server Console Client ...
                var scc = new ServerConsoleClient();
                server.CreateClient(scc, parent).SetDisconnectedHandler(
                    (session, args) => { scc.StartConnectionEstablishment(); });
                // The Client Console Client ...
                var ccc = new ClientConsoleClient();
                server.CreateClient(ccc, parent);
                var bo1 = new CccBo1();
                server.CreateClient(bo1, ccc);
                server.CreateClient(new CccBo2(), bo1);

                if (!server.Start())
                {
                    throw new Exception("Cannot start UA Server without errors!");
                }

                var startTs = DateTimeHelper.currentTimeMillis();
                while (true)
                {
                    Thread.Sleep(1000);
/*
                    var runtime = DateTimeHelper.currentTimeMillis() - startTs;
                    if (15000 < runtime && runtime < 17000)
                    {
                        var cl = server.AddClient<ClientConsoleClient>();
                        Logger.Info($"{cl} added.");
                    }
                    else if (runtime > 17000)
                    {
                        var registeredClients = server.RegisteredClients();
                        if (registeredClients.Count <= 1) continue;
                        Console.Out.WriteLine($"Registered Business Object count == {registeredClients.Count}");
                        var lbo = registeredClients.Last();
                        server.RemoveClient(lbo.BoId);
                    }
*/
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "Error while starting the MultiClientConsole ...");
                Logger.Info("Press <enter> to exit the program.");
                Console.ReadLine();
            }
            finally
            {
                // Stop the server.
                server?.Stop();
            }
        }
    }
}