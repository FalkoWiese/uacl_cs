using System;
using UaclClient;
using UaclServer;
using UaclUtils;

namespace ServerConsole
{
    [UaObject]
    public class BusinessLogic : LocalProxy
    {
        public BusinessLogic()
        { }

        public BusinessLogic(ConnectionInfo info) : base(typeof(BusinessLogic).Name)
        { }

        private enum JobState
        {
            None = 0,
            Initialized = 1,
            Running = 2,
            Finished = 3,
            Error = 4,
        }

        [UaclInsertState]
        [UaMethod]
        public int GetInteger(object state, string value)
        {
            int val;
            Int32.TryParse(value, out val);
            return val;
        }

        [UaMethod]
        public bool CalculateJob(string name, int state)
        {
            var result = true;

            switch (state)
            {
                case (int)JobState.None:
                    BoState = $"Job {name} not exists!";
                    break;
                case (int)JobState.Initialized:
                    BoState = $"Job {name} is Initialized!";
                    break;
                case (int)JobState.Running:
                    BoState = $"Job {name} is Running!";
                    break;
                case (int)JobState.Finished:
                    BoState = $"Job {name} is Finished!";
                    break;
                default:
                    BoState = $"Job {name} has an Error!";
                    result = false;
                    break;
            }

            Logger.Info($"Result => '{result}', State => '{BoState}'");
            return result;
        }

        [UaMethod("JobStates")]
        public string States()
        {
            return @"{""states"": [""None"",""Initialized"",""Running"",""Finished"",""Error""]}";
        }

        [UaVariable]
        public string BoState
        {
            get { return _boState; }
            set
            {
                _boState = value;
                Logger.Trace($"Wrote property State to '{value}'.");
            }
        }

        private string _boState;

        public void ChangeState(string newState)
        {
            Call("BoState", newState);
        }
    }
}
