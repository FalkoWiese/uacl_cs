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
                            ChangeState(count);
                            ChangeState((float)count/100);
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

        [UaclInsertState]
        [UaMethod]
        public int GetInteger(object state, string value)
        {
            int val;
            Int32.TryParse(value, out val);
            return val;
        }

        [UaMethod]
        public int? GetNullInteger()
        {
            return null;
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

        [UaMethod]
        public string GetNullString()
        {
            return null;
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

        [UaVariable]
        public int IntBoState
        {
            get { return _intBoState; }
            set
            {
                _intBoState = value;
                Logger.Trace($"Wrote property IntState to '{value}'.");
            }
        }

        private int _intBoState;

        [UaVariable]
        public float FloatBoState
        {
            get { return _floatBoState; }
            set
            {
                _floatBoState = value;
                Logger.Trace($"Wrote property FloatState to '{value}'.");
            }
        }

        private float _floatBoState;

        public void ChangeState(string newState)
        {
            Call("BoState", newState);
        }

        public void ChangeState(int newState)
        {
            Call("IntBoState", newState);
        }

        public void ChangeState(float newState)
        {
            Call("FloatBoState", newState);
        }
    }
}
