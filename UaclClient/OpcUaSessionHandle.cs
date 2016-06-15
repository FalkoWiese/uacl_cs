﻿using System;
using System.Collections.Generic;
using System.Threading;
using UaclUtils;
using UnifiedAutomation.UaClient;

namespace UaclClient
{
    public class OpcUaSessionHandle : IDisposable
    {
        public OpcUaSessionHandle(OpcUaSession session)
        {
            Session = session;
            MonitoredItems = new List<MonitoredItem>();
        }

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
                ClientSubscription().DeleteMonitoredItems(MonitoredItems);
                ClientSubscription().Delete(new RequestSettings { OperationTimeout = 5000 });
                _clientSubscription = null;
                if (Session.ConnectionStatus == ServerConnectionStatus.Connected)
                {
                    Session.Disconnect();
                }
                Session.Dispose();
                Session = null;
            }
            catch (Exception e)
            {
                ExceptionHandler.Log(e, "Error while session is disposing.");
            }
        }
    }
}