using System;
using UaclServer;
using UaclUtils;

namespace ServerConsole
{
    [UaObject]
    public class BusinessLogic
    {
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
                    State = $"Job {name} not exists!";
                    break;
                case (int)JobState.Initialized:
                    State = $"Job {name} is Initialized!";
                    break;
                case (int)JobState.Running:
                    State = $"Job {name} is Running!";
                    break;
                case (int)JobState.Finished:
                    State = $"Job {name} is Finished!";
                    break;
                default:
                    State = $"Job {name} has an Error!";
                    result = false;
                    break;
            }

            Logger.Info($"Result => '{result}', State => '{State}'");
            return result;
        }

        [UaMethod("JobStates")]
        public string States()
        {
            return @"{""states"": [""None"",""Initialized"",""Running"",""Finished"",""Error""]}";
        }

        [UaVariable]
        public string State { get; set; }
    }
}
