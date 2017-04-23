using System;
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
                var factory = new UaFactory(server);

                var parent = factory.CreateUaObject<MultiClientHost>();

                foreach (var remoteObject in new RemoteObject[]
                    {new ServerConsoleClient("localhost", 48030), new ClientConsoleClient("localhost", 48040)})
                {
                    factory.CreateUaObject(remoteObject, parent)
                        .SetDisconnectedHandler(
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
                        var cl = factory.AddUaObject<ClientConsoleClient>();
                        Logger.Info($"{cl} added.");
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