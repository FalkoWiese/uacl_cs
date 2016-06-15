using System;
using System.Collections.Generic;
using System.Threading;
using UaclUtils;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class OpcUaSessionHandle
    {
        public OpcUaSessionHandle(OpcUaSession session, Thread handler = null)
        {
            Handler = handler;
            Session = session;
            CreateSubscription();
        }

        private void CreateSubscription()
        {
            try
            {
                if (ClientSubscription != null)
                {
                    ClientSubscription.Delete(new RequestSettings {OperationTimeout = 5000});
                    ClientSubscription = null;
                }

                ClientSubscription = new Subscription(Session)
                {
                    PublishingInterval = 0,
                    MaxKeepAliveTime = 5000,
                    Lifetime = 600000,
                    MaxNotificationsPerPublish = 0,
                    Priority = 0,
                    PublishingEnabled = true
                };

                ClientSubscription.Create(new RequestSettings { OperationTimeout = 10000 });

                DataChangeHandlerAvailable = false;
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "Cannot create a SUBSCRIPTION for a SESSION object!");
            }
        }

        public void SetDataChangeHandler(DataChangedEventHandler eventHandler)
        {
            if (DataChangeHandlerAvailable) return;
            ClientSubscription.DataChanged += eventHandler;
            DataChangeHandlerAvailable = true;
        }

        private bool DataChangeHandlerAvailable { get; set; }

        public Subscription ClientSubscription { get; set; }

        public Thread Handler { get; set; }

        public OpcUaSession Session { get; set; }
    }
}