using System;
using System.Collections.Generic;
using System.Threading;
using UaclUtils;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public sealed class OpcUaSessionHandle : IDisposable
    {
        public OpcUaSessionHandle(OpcUaSession session)
        {
            Session = session;
            MonitoredItems = new List<MonitoredItem>();
            Timeout = false;
        }

        public bool Timeout { get; set; }

        private Subscription CreateSubscription()
        {
            try
            {
                Subscription s = new Subscription(Session)
                {
                    PublishingInterval = 0,
                    MaxKeepAliveTime = 5000,
                    Lifetime = 600000,
                    MaxNotificationsPerPublish = 0,
                    Priority = 0,
                    PublishingEnabled = true
                };

                s.Create(new RequestSettings {OperationTimeout = 10000});

                DataChangeHandlerAvailable = false;

                return s;
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "Cannot create a SUBSCRIPTION for a SESSION object!");
            }

            return null;
        }

        public void SetDataChangeHandler(DataChangedEventHandler eventHandler)
        {
            if (DataChangeHandlerAvailable) return;
            ClientSubscription().DataChanged += eventHandler;
            DataChangeHandlerAvailable = true;
        }

        private bool DataChangeHandlerAvailable { get; set; }

        public Subscription ClientSubscription()
        {
            return _clientSubscription ?? (_clientSubscription = CreateSubscription());
        }

        Subscription _clientSubscription;

        public OpcUaSession Session { get; set; }

        public List<MonitoredItem> MonitoredItems { get; set; }

        public void Dispose()
        {
            try
            {
                if (MonitoredItems.Count > 0)
                {
                    ClientSubscription().DeleteMonitoredItems(MonitoredItems);
                }
                ClientSubscription().Delete(new RequestSettings { OperationTimeout = 5000 });
                _clientSubscription = null;
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "Error while session is disposing.");
            }
            finally 
            {
                if (Session != null)
                {
                    if (Session.ConnectionStatus == ServerConnectionStatus.Connected)
                    {
                        Session.Disconnect();
                    }
                    Session.ConnectionStatusUpdate -= StatusChangedHandler;
                    StatusChangedHandler = null;
                    Session.Dispose();
                    Session = null;
                }
            }
        }

        private ServerConnectionStatusUpdateEventHandler StatusChangedHandler { get; set; }

        public bool AddStatusChangedHandler(ServerConnectionStatusUpdateEventHandler statusChangedCallback)
        {
            if (StatusChangedHandler != null) return false;

            StatusChangedHandler = statusChangedCallback;
            Session.ConnectionStatusUpdate += StatusChangedHandler;
            return true;
        }
    }
}