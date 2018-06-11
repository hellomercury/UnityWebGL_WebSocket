using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LitJson;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TestWebSocketServer
{
    public partial class Form1 : Form
    {
        public static Form1 instance;

        public Form1()
        {
            InitializeComponent();
            instance = this;
        }

        WebSocketServer server = null;

        private void start_Click(object sender, EventArgs e)
        {
            PrintText("start listening...");
            int spIndex = address.Text.LastIndexOf('/');
            string addr = address.Text.Substring(0, spIndex);
            string path = address.Text.Substring(spIndex);
            server = new WebSocketServer(addr);
            server.WaitTime = TimeSpan.FromSeconds(2);
            server.AddWebSocketService<MessageHandle>(path);
            server.Start();
        }

        private delegate void DelegatePrintText(string str, System.Drawing.Color color);
        public void PrintText(string str, System.Drawing.Color color)
        {
            if (receive.InvokeRequired)
            {
                DelegatePrintText du = new DelegatePrintText(PrintText);
                receive.Invoke(du, new object[] { str, color });
            }
            else
            {
                RtbAppend(str, color);
            }
        }

        public void PrintText(string str)
        {
            PrintText(str + "\n", System.Drawing.Color.Black);
        }

        private void RtbAppend(string strInput, System.Drawing.Color fontColor)
        {
            int p1 = receive.TextLength;            //取出未添加时的字符串长度
            receive.AppendText(strInput);           //保留每行的所有颜色 
            int p2 = strInput.Length;               //取出要添加的文本的长度 
            receive.Select(p1, p2);                 //选中要添加的文本 
            receive.SelectionColor = fontColor;     //设置要添加的文本的字体色 
            receive.Focus();
        }

        private void address_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class MessageHandle : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Form1.instance.PrintText("Client Connected: " + ID);
            //SendMessage("Connect at " + DateTime.Now);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                string msg = Encoding.UTF8.GetString(e.RawData);

                JsonData data = JsonMapper.ToObject(msg);
                Console.WriteLine(data);
                Form1.instance.PrintText("Receive From :" + ID + "\n" + data.ToJson());
                JsonData returnData = new JsonData();
                
                switch (data["name"].ToString())
                {
                    case "Q3RDAuthWebGL":

                        break;

                    case "Q3RDWSEnter":
                        returnData["name"] = "R3RDAuth";
                        returnData["code"] = "0";
                        returnData["echo"] = data["echo"];
                        returnData["sign"] = "abcde12345";
                        returnData["time"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        break;

                    case "QDataUpdate":
                        returnData["name"] = "RDataUpdate";
                        returnData["code"] = "0";
                        returnData["echo"] = data["echo"];
                        returnData["gold"] = 10000;
                        returnData["score"] = 10000;
                        returnData["world"] = 10000;
                        returnData["sub_world"] = 10000;
                        returnData["level"] = 10000;
                        returnData["time"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        break;

                    case "QCustomsDataPut":
                        returnData["name"] = "RCustomsDataPut";
                        returnData["code"] = "0";
                        returnData["echo"] = data["echo"];
                        returnData["time"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        break;

                    case "QPing":
                        returnData["name"] = "RPing";
                        returnData["code"] = "0";
                        returnData["echo"] = data["echo"];
                        returnData["time"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                        break;
                }

                Form1.instance.PrintText("return :" + returnData.ToJson());
                SendMessage(returnData.ToJson());
            }
            catch (Exception msg)
            {
                Form1.instance.PrintText("return :" + msg.Message + "\n" + msg.StackTrace);
            }

        }

        protected override void OnClose(CloseEventArgs e)
        {
            Form1.instance.PrintText("Client Closed :" + ID);
            Sessions.CloseSession(ID);
        }

        public void SendMessage(string msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            Sessions.SendToAsync(data, ID, (a) => { });
        }
    }
}
