using UaclServer;

namespace ServerConsole
{
    [UaObject]
    public class BusinessLogic
    {
        [UaMethod]
        public bool CalculateJob(string name, int id)
        {
            System.Console.WriteLine($"Job Started ... {name} ({id})");
            return true;
        }

        [UaVariable]
        public string State { get; set; }
    }
}
