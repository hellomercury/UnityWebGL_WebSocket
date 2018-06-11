using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;
using WebSocket = BestHTTP.WebSocket.WebSocket;

namespace Net
{
    public class NetManager : MonoBehaviour
    {
        public static NetManager Instance { get; private set; }


        private readonly string url = "https://crossy.fbh5.live/3rd";
        private Uri socketUri;
        private string serverSession;
        private string serverSign;
        private string serverTime;

        [SerializeField]
        private string facebookToken = string.Empty;
        private WebSocket webSocket;

        private Queue<NetMsg> netMsgQueue;

        void Awake()
        {
            if (null == Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            netMsgQueue = new Queue<NetMsg>(NetConfig.MAX_CACHE_NET_MSG_COUNT_USHORT);
            socketState = SocketState.UnOpened;

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //if (!FB.IsInitialized)
            //{
            //    FB.Init(() =>
            //    {
            //        Debug.Log("[FB]    Init succeed.");
            //        FBLogin.PromptForLoginInit(() =>
            //        {
            //            Debug.Log("[FB]    Login init succeed.");

            //            sw.Stop();
            //            Debug.Log("[FB]    Use time = " + sw.ElapsedMilliseconds);

            //            int retryTime = 0;
            //            Action<string> failAction = null;

            //            failAction = errorMsg =>
            //            {
            //                Debug.LogError(errorMsg);

            //                if (retryTime < NetConfig.MAX_RETRY_TIMES_USHORT)
            //                {
            //                    StartCoroutine(GetSocketAddress(OpenSocket, failAction));
            //                    ++retryTime;
            //                }
            //                else Debug.LogError("[Http]    Get Socket Address Failed.");
            //            };

            //            //NetDispatcher.Instance.RegisterNetEvent(NetEventEnum.Q3RDWSEnter, EnterGame);

            //            StartCoroutine(GetSocketAddress(OpenSocket, failAction));
            //        });
            //    });
            //}

            socketUri = new Uri("ws://127.0.0.1:8759/test");
            OpenSocket(socketUri);
        }

        private IEnumerator GetSocketAddress(Action<Uri> InSuccessAction, Action<string> InFailAction)
        {
            Debug.Log("[Http]    Get Socket Address.");
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(facebookToken)) Debug.LogError("请输入FaceBook测试Token.");
#else
            facebookToken = Facebook.Unity.AccessToken.CurrentAccessToken.TokenString;
#endif
            JsonData securityJson = new JsonData();
            securityJson["token"] = facebookToken;
            JsonData authJson = new JsonData();
            authJson["type"] = "facebook_tmp";
            authJson["security_info"] = securityJson;
            authJson["echo"] = string.Empty;
            authJson["name"] = "Q3RDAuthWebGL";

            //Debug.Log("[Http]    Req = " + authJson.ToJson());
            //request.RawData = Encoding.Default.GetBytes(authJson.ToJson());
            //request.Send();

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            byte[] postBytes = Encoding.Default.GetBytes(authJson.ToJson());
            Debug.Log("[Http]    Req = " + authJson.ToJson());

            request.uploadHandler = new UploadHandlerRaw(postBytes);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.Send();


            if (request.isError)
            {
                Debug.Log(request.error);
            }
            else if (200 == request.responseCode)
            {
                JsonData result = JsonMapper.ToObject(request.downloadHandler.text);

                if (result["code"].ToString() == "0")
                {
                    serverSession = result["session"].ToString();
                    serverSign = result["sign"].ToString();
                    serverTime = result["time"].ToString();
                    socketUri = new Uri(result["gw_game_server"].ToString());

                    InSuccessAction.Invoke(socketUri);
                }
            }
            else
            {
                Debug.LogError("Token 可能已过期，请更新。");
            }
        }

        private void EnterGame()
        {
            JsonData jsonData = new JsonData();
            jsonData["appid"] = "Jumpit";
            jsonData["time"] = serverTime;
            jsonData["sign"] = NetUtility.GetStringMd5("Jumpit" + serverTime + serverSign);
            jsonData["session"] = serverSession;
            jsonData["pver"] = "1.0.0";

            Request(NetProtocolEnum.Q3RDWSEnter, jsonData);
        }

        #region Client
        /// <summary>
        /// 不需要把协议名加入到InProtocolData中，NetMsg类会自动添加name字段
        /// </summary>
        /// <param name="InNetProtocolEnum">协议类型</param>
        /// <param name="InProtocolData">协议参数</param>
        public void Request(NetProtocolEnum InNetProtocolEnum, JsonData InProtocolData)
        {
            Enqueue(new NetMsg(InNetProtocolEnum, InProtocolData));
        }
        #endregion

        #region MsgQueue

        private void Enqueue(NetMsg InMsg)
        {
            Debug.Log("[Socket Utility]    消息入队 = " + InMsg);

            netMsgQueue.Enqueue(InMsg);

            UpdateMsgState();
        }

        private void Requeue(NetMsg InMsg)
        {
            Debug.Log("[Socket Utility]    消息重新入队 = " + InMsg);

            InMsg.Reset();

            netMsgQueue.Enqueue(InMsg);
        }

        private NetMsg Dequeue()
        {
            NetMsg topMsg = netMsgQueue.Dequeue();

            Debug.Log("[Socket Utility]    消息出队 = " + topMsg);

            UpdateMsgState();

            return topMsg;
        }



        private void ClearQueue()
        {
            if (netMsgQueue.Count > 0) netMsgQueue.Clear();
        }
        #endregion

        #region State
        private int retryTime = 0;
        private enum SocketState
        {
            UnOpened,
            Opened,
            Idle,
            Busy,
            TimeOut,
            Error,
            Closed
        }

        private SocketState socketState;

        private void UpdateMsgState()
        {
            Debug.Log("[Socket Utility]    UpdateMsgState = " + socketState);
            if (socketState == SocketState.Idle)
            {
                if (netMsgQueue.Count > 0)
                {
                    NetMsg topMsg = netMsgQueue.Peek();
                    if (!topMsg.WhetherWaiting)
                    {
                        if (!topMsg.SendToSocket(webSocket))
                        {
                            Debug.Log("[Socket Utility]    重试超时 = " + topMsg);
                            Requeue(netMsgQueue.Dequeue());

                            UpdateSocketState(SocketState.TimeOut);
                        }
                        else UpdateSocketState(SocketState.Busy);
                    }
                    else
                    {
                        Debug.Log("[Socket Utility]    等待返回 = " + topMsg);
                    }
                }
            }
        }


        private void UpdateSocketState(SocketState InSocketState)
        {
            socketState = InSocketState;
            switch (InSocketState)
            {
                case SocketState.Opened:
                    UpdateSocketState(SocketState.Idle);
                    retryTime = 0;
                    break;

                case SocketState.Idle:
                    UpdateMsgState();
                    break;
                    
                case SocketState.TimeOut:
                case SocketState.Error:
                    ReConnect();
                    break;

                case SocketState.Closed:
                    if(Application.isPlaying) ReConnect();
                    break;
            }

            Debug.Log("[Socket Utility]    UpdateSocketState = " + InSocketState);
        }

        private void ReConnect()
        {
            if (retryTime < NetConfig.MAX_RETRY_TIMES_USHORT)
            {
                OpenSocket(socketUri);

                ++retryTime;
            }
            else Debug.LogError("Net Disconnection.");
        }

        #endregion

        #region Socket

        public void OpenSocket(Uri InUri)
        {
            Debug.Log("[Socket Utility]    EnterGame = " + (webSocket == null || webSocket.IsOpen));
            if (webSocket == null || !webSocket.IsOpen)
            {
                Debug.Log("[Socket Utility]    连接Socket.");
                WebSocket socket = new WebSocket(InUri);
                socket.OnOpen += openSocket =>
                {
                    Debug.Log("[Socket Utility]    连接Socket成功.");
                    webSocket = openSocket;
                    webSocket.OnBinary += OnBinaryMsgReceived;
                    webSocket.OnError += OnWebSocketError;
                    webSocket.OnClosed += OnWebSocketClosed;

                    UpdateSocketState(SocketState.Idle);


                    NetDispatcher.Instance.RegisterNetEvent(NetProtocolEnum.Q3RDWSEnter, StartHeartbeat);
                    EnterGame();
                };

                //socket.StartPingThread = true;
                socket.Open();
            }
        }

        private void OnBinaryMsgReceived(WebSocket InWebSocket, byte[] InBinaryData)
        {
            string oriMsg = Encoding.Default.GetString(InBinaryData);
            Debug.Log("[Socket Utility]    收到服务器消息 = " + oriMsg);

            JsonData oriJson = JsonMapper.ToObject(oriMsg);
            int msgKey = Convert.ToInt32(oriJson["echo"].ToJson());

            if (msgKey > 0)
            {
                if (netMsgQueue.Count > 0)
                {
                    if (netMsgQueue.Peek().Key == msgKey)
                    {
                        string code = oriJson["code"].ToString();

                        NetMsg msg = Dequeue();
                        switch (code)
                        {
                            case NetConfig.NET_BACK_SUCCEED_CODE_S:
                                msg.InvokeCallback(NetResultEnum.Done, oriJson);
                                break;

                            default:
                                Debug.Log("[Socket Utility]    ->resp<- error code = " + code + "\nerror = " + oriJson["error"]);
                                msg.InvokeCallback(NetResultEnum.Error, null);
                                break;
                        }
                    }
                }
            }
            else if (msgKey == NetConfig.HEART_BEAT_MSG_CODE_I)
            {
                ReceiveHeartBeatMsg(oriJson);
            }
            else
            {
                Debug.Log("[Socket Utility]    Unknow message code.");
            }
        }

        private void OnWebSocketError(WebSocket InWebSocket, Exception InException)
        {
            string errorMsg = string.Empty;
#if !UNITY_WEBGL
                if (InWebSocket.InternalRequest.Response != null)
                {
                    errorMsg = "Status Code from Server: " + InWebSocket.InternalRequest.Response.StatusCode + "\nMessage: " + InWebSocket.InternalRequest.Response.Message + "\n";
                }
#else
            errorMsg += "-An error occured: " + (null == InException ? "Unknown Error!" : InException.Message) + "\n" + errorMsg;
#endif

            Debug.Log("[Socket Utility]    " + errorMsg);

            UpdateSocketState(SocketState.Error);
        }

        private void OnWebSocketClosed(WebSocket InWebSocket, ushort InCode, string InMsg)
        {
            string errorMsg = "Unknow code.";
            switch (InCode)
            {
                case 1000:
                    errorMsg = "Socket 超时断开。";
                    break;

                case 1011:
                    errorMsg = "协议名称错误，请校验API文档。";
                    break;
            }

            Debug.Log("[Socket Utility]    Closed.\nCode = " + InCode + "\nErrorMsg = " + errorMsg + "\nOriMsg = " + InMsg);

            UpdateSocketState(SocketState.Closed);
        }

        #endregion

        #region Heartbeat

        private byte[] heartReqData;

        public void StartHeartbeat()
        {
            Debug.Log("[Socket Utility]    开启心跳.");

            InvokeRepeating("Heartbeat", 0, NetConfig.HEART_BEAT_SPACE_TIME_I);

            JsonData heartReqJson = new JsonData();
            heartReqJson["name"] = NetProtocolEnum.QPing.ToString();
            heartReqJson["echo"] = NetConfig.HEART_BEAT_MSG_CODE_I;
            heartReqData = Encoding.Default.GetBytes(heartReqJson.ToJson());
        }

        void ReceiveHeartBeatMsg(JsonData InJsonDataArray)
        {
            Debug.Log("[Socket Utility]    心跳返回.");
        }

        private void Heartbeat()
        {
            //if (isSocketLoopAlreadyOpen)
            {
                Debug.Log("[Socket Utility]    发送心跳.");
                webSocket.Send(heartReqData);
            }
            //else
            {
                Debug.Log("[Socket Utility]    网络断开.");
            }
        }

        #endregion
    }
}
