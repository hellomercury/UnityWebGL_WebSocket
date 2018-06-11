using System;
using System.Collections.Generic;
using LitJson;
using UnityEngine.Assertions;

namespace Net
{
    public class NetDispatcher
    {
        private static NetDispatcher instance;

        public static NetDispatcher Instance
        {
            get
            {
                if(null == instance) instance = new NetDispatcher();
                return instance;
            }
        }

        private Dictionary<NetProtocolEnum, Action<bool, JsonData>> netActionDicts;
        private Dictionary<NetProtocolEnum, Action> netActionWithoutDataDicts; 

        private NetDispatcher()
        {
            netActionDicts = new Dictionary<NetProtocolEnum, Action<bool, JsonData>>(new NetEventEnumCpr());
            netActionWithoutDataDicts = new Dictionary<NetProtocolEnum, Action>(new NetEventEnumCpr());
        }

        public void RegisterNetEvent(NetProtocolEnum InNetProtocol, Action<bool, JsonData> InEventAction)
        {
            Assert.IsNotNull(InEventAction, "No function can be invoked.");
            
            if (netActionDicts.ContainsKey(InNetProtocol)) netActionDicts[InNetProtocol] += InEventAction;
            else netActionDicts.Add(InNetProtocol, InEventAction);
        }

        public void RegisterNetEvent(NetProtocolEnum InNetProtocol, Action InEventAction)
        {
            Assert.IsNotNull(InEventAction, "No function can be invoked.");

            if (netActionWithoutDataDicts.ContainsKey(InNetProtocol)) netActionWithoutDataDicts[InNetProtocol] += InEventAction;
            else netActionWithoutDataDicts.Add(InNetProtocol, InEventAction);
        }

        public void UnRegisterNetEvent(NetProtocolEnum InNetProtocol, Action<bool, JsonData> InEventAction)
        {
            if(netActionDicts.ContainsKey(InNetProtocol)) netActionDicts[InNetProtocol] -= InEventAction;
        }
        
        public void UnRegisterNetEvent(NetProtocolEnum InNetProtocol, Action InEventAction)
        {
            if (netActionWithoutDataDicts.ContainsKey(InNetProtocol)) netActionWithoutDataDicts[InNetProtocol] -= InEventAction;
        }
        
        public void NetEventTrigger(NetProtocolEnum InNetProtocol, bool InNetResult, JsonData InNetMsg)
        {
            Action<bool, JsonData> eventAction;
            if (netActionDicts.TryGetValue(InNetProtocol, out eventAction))
            {
                eventAction.Invoke(InNetResult, InNetMsg);

                netActionDicts.Remove(InNetProtocol);
            }

            if (InNetResult)
            {
                Action eventWithoutDataAction;
                if (netActionWithoutDataDicts.TryGetValue(InNetProtocol, out eventWithoutDataAction))
                {
                    eventWithoutDataAction.Invoke();

                    netActionWithoutDataDicts.Remove(InNetProtocol);
                }
            }
        }
    }
}

