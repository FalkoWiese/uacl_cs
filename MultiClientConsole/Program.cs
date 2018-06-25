using System;
using System.Linq;
using System.Threading;
using UaclClient;
using UaclServer;
using UaclUtils;

namespace MultiClientConsole
{
    delegate ModuleDescription DescFactory(AppDescription app, string suffix);

    class Program
    {
        static void Main()
        {
            InternalServer server = null;
            try
            {
                var weberScannerDesc = new AppDescription
                {
                    BaseUrl = "https://weber.adamos-dev.com",
                    DeviceId = "206293"
                };
                var weberSlicerDesc = new AppDescription
                {
                    BaseUrl = "https://weber.adamos-dev.com",
                    DeviceId = "206292"
                };

                var weber2ScannerDesc = new AppDescription
                {
                    BaseUrl = "https://weber2.adamos-dev.com",
                    DeviceId = "198123",
                };
                var weber2SlicerDesc = new AppDescription
                {
                    BaseUrl = "https://weber2.adamos-dev.com",
                    DeviceId = "198084",
                };


                DescFactory descFactory = delegate(AppDescription app, string suffix)
                {
                    var desc = new ModuleDescription
                    {
                        UrlPrefix = app.BaseUrl,
                        Suffix = suffix,
                        AdamosId = app.DeviceId,
                        Username = "<username>",
                        Password = "<password>"
                    };
                    return desc;
                };

                server = new MultiClientServer();

                var slicer = new WpcModuleClient("172.20.30.231", 4841, "M01_SLICER")
                {
                    Desc = descFactory(weber2SlicerDesc, "event/events")
                };
                server.CreateClient(slicer)
                    .SetDisconnectedHandler((session, args) => { slicer.StartConnectionEstablishment(); });

                var weights = new WpcTrackClient("172.20.30.231", 4841, "M01_SLICER")
                {
                    Desc = descFactory(weber2SlicerDesc, "measurement/measurements")
                };
                server.CreateClient(weights)
                    .SetDisconnectedHandler((session, args) => { weights.StartConnectionEstablishment(); });

                var scanner = new WpcModuleClient("172.20.30.231", 4841, "M00_CLASSIC_PDM_SYSTEM")
                {
                    Desc = descFactory(weber2ScannerDesc, "event/events")
                };

                server.CreateClient(scanner)
                    .SetDisconnectedHandler((session, args) => { scanner.StartConnectionEstablishment(); });

                if (!server.Start())
                {
                    throw new Exception("Cannot start UA Server without errors!");
                }

                while (true)
                {
                    Thread.Sleep(1000);
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