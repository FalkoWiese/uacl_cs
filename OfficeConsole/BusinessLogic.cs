using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private string _boState;

        public void ChangeState(string newState)
        {
            Call("BoState", newState);
        }
    }
}

