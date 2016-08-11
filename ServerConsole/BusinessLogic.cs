using System;
using System.Threading;
using UaclServer;
using UaclUtils;

namespace ServerConsole
{
    [UaObject]
    public class BusinessLogic : ServerSideUaProxy
    {
        private Thread WorkerThread { get; set; }

        public BusinessLogic()
        {
        }

        private enum JobState
        {
            None = 0,
            Initialized = 1,
            Running = 2,
            Finished = 3,
            Error = 4,
        }

        [UaMethod]
        public void ToggleValueChangeThread()
        {
            if (WorkerThread == null)
            {
                WorkerThread = new Thread(() =>
                {
                    try
                    {
                        int count = 0;
                        while (true)
                        {
                            ChangeState($"{count++}");
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
                WorkerThread.Start();
            }
            else
            {
                WorkerThread.Interrupt();
                WorkerThread = null;
            }
        }

        [UaMethod]
        public void FireSimpleEvent()
        {
            Guid id = Guid.NewGuid();
            FireEvent("FireSimpleEvent", id);
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
                case (int) JobState.None:
                    BoState = $"Job {name} not exists!";
                    break;
                case (int) JobState.Initialized:
                    BoState = $"Job {name} is Initialized!";
                    break;
                case (int) JobState.Running:
                    BoState = $"Job {name} is Running!";
                    break;
                case (int) JobState.Finished:
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
