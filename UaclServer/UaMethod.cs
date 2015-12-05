namespace UaclServer
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class UaMethod : System.Attribute
    {
        public string Name { get; private set; }

        public UaMethod(string name=null)
        {
            Name = name;
        }
    }
}
