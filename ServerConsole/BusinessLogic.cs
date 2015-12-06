using UaclServer;
using UaclUtils;

namespace ServerConsole
{
    [UaObject]
    public class BusinessLogic
    {
        [UaMethod]
        public bool CalculateJob(string name, int state)
        {
            Logger.Info($"Job Started ... {name} ({state})");
            State = $"{state}";
            return true;
        }

        [UaVariable]
        public string State
        {
            get;
            set;
        }
    }
}
