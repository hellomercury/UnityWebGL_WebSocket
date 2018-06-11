using System.Text;
using BestHTTP.WebSocket;
using LitJson;
using UnityEngine.Assertions;

namespace Net
{
    public class NetMsg
    {
        private static int singletonKey = 1;

        public int Key { get; }

        public NetProtocolEnum NetProtocolType { get; }

        private byte[] binaryMsg;
        
        public bool WhetherWaiting { get; private set; }

        private bool wantRawData;

        private ushort retryTimes;

        public NetMsg(NetProtocolEnum InNetProtocolEnum, JsonData InMsgJsonData)
        {
            Assert.IsNotNull(InMsgJsonData);

            NetProtocolType = InNetProtocolEnum;
            InMsgJsonData["name"] = InNetProtocolEnum.GetNetProtocolName();

            Key = singletonKey;
            InMsgJsonData["echo"] = Key;
            binaryMsg = Encoding.Default.GetBytes(InMsgJsonData.ToJson());
            
            WhetherWaiting = false;

            if (++singletonKey == int.MaxValue) singletonKey = 1;
        }

        public NetMsg(NetProtocolEnum InNetProtocolEnum, JsonData InMsgJsonData, int InKey)
        {
            Assert.IsNotNull(InMsgJsonData);

            Key = InKey;
            InMsgJsonData["msgMark"] = Key;
            NetProtocolType = InNetProtocolEnum;
            InMsgJsonData["name"] = InNetProtocolEnum.GetNetProtocolName();
            binaryMsg = Encoding.Default.GetBytes(InMsgJsonData.ToJson());

            WhetherWaiting = false;
        }

        public void InvokeCallback(NetResultEnum InResult, JsonData InRespJson)
        {
            WhetherWaiting = false;

            NetDispatcher.Instance.NetEventTrigger(NetProtocolType, InResult == NetResultEnum.Done, InRespJson);
        }

        public void InterruptWaiting()
        {
            WhetherWaiting = false;
        }

        public bool SendToSocket(WebSocket InWebSocket)
        {
            if (retryTimes < NetConfig.MAX_RETRY_TIMES_USHORT)
            {
                WhetherWaiting = true;

                ++retryTimes;

                InWebSocket.Send(binaryMsg);

                return true;
            }
            else return false;
        }
        
        public void Reset()
        {
            retryTimes = 0;
            WhetherWaiting = false;
        }

        public override string ToString()
        {
            string log = Encoding.Default.GetString(binaryMsg);
            if (retryTimes > 1) log += "\nRetry time = " + retryTimes;
            return log;
        }
    }
}