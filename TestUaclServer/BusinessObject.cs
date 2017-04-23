using UaclServer;

namespace TestUaclServer
{
    [UaObject]
    public class BusinessObject
    {
        public bool Compare(int i, int j)
        {
            return i != j;
        }
    }
}
