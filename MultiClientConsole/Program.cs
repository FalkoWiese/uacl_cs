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

                var parent = server.CreateClient<MultiClientHost>();

                foreach (var remoteObject in new RemoteObject[]
                    {new ServerConsoleClient("localhost", 48030), new ClientConsoleClient("localhost", 48040)})
                {
                    server.CreateClient(remoteObject, parent).SetDisconnectedHandler(
                            (session, args) => { remoteObject.StartConnectionEstablishment(); });
                }


                if (!server.Start())
                {
                    throw new Exception("Cannot start UA Server without errors!");
                }

                var startTs = DateTimeHelper.currentTimeMillis();
                while (true)
                {
                    Thread.Sleep(1000);
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