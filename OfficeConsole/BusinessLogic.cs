using System;
using System.Threading;
using UaclServer;
using UaclUtils;

namespace OfficeConsole
{
    [UaObject]
    public class BusinessLogic : ServerSideUaProxy
    {
        private Thread WorkerThread { get; set; }

        [UaMethod]
        public string GetStatistics(int id)
        {
            if(id < 0) throw new Exception("Cannot find statistics with an ID lesser than zero!");
            return @"{ ""statistics"": [{}, {}, {}]}";
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
                            ChangeIntState(count);
                            ChangeFloatState((float)count/100);
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

        [UaVariable]
        public float FloatBoState
        {
            get { return _floatBoState; }
            set
            {
                _floatBoState = value;
                Logger.Trace($"Wrote property IntState to '{value}'.");
            }
        }

        private string _boState;
        private int _intBoState;
        private float _floatBoState;

        public void ChangeState(string newState)
        {
            Call("BoState", newState);
        }

        public void ChangeIntState(int newState)
        {
            Call("IntBoState", newState);
        }

        public void ChangeFloatState(float newState)
        {
            Call("FloatBoState", newState);
        }
    }
}

