using UaclServer;
using UaclUtils;

namespace ServerConsole
{
    [UaObject]
    public class BusinessLogic
    {
        [UaMethod]
        public bool CalculateJob(string name, int id)
        {
            Logger.Info($"Job Started ... {name} ({id})");
            return true;
        }

        [UaVariable]
        public string State { get; set; }
    }
}
