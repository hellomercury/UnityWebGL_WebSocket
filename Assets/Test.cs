using LitJson;
using Net;
using UnityEngine;

public class Test : MonoBehaviour
{
    void OnGUI()
    {
        GUI.skin.button.fontSize = 64;

        if (GUILayout.Button("QDataUpdate"))
        {
            JsonData updateData = new JsonData();
            updateData["photo"] = "www.baidu.com";
            updateData["player_name"] = "szn";
            JsonData friendsArray = new JsonData();
            friendsArray.Add(new JsonData());
            updateData["friends"] = friendsArray;
            
            NetDispatcher.Instance.RegisterNetEvent(NetProtocolEnum.QDataUpdate,
                (result, data) =>
                {
                   Debug.LogError("Resp QDataUpdate msg = " + result + "\n" + data.ToJson());
                });
            NetManager.Instance.Request(NetProtocolEnum.QDataUpdate, updateData);
        }

        if (GUILayout.Button("QCustomsDataPut"))
        {
            JsonData cusData = new JsonData();
            cusData["gold"] = 10000;
            cusData["score"] = 10001;
            cusData["world"] = 10002;
            cusData["sub_world"] = 10003;
            cusData["level"] = 10004;

            NetDispatcher.Instance.RegisterNetEvent(NetProtocolEnum.QCustomsDataPut,
                (result, data) =>
                {
                    Debug.LogError("Resp QCustomsDataPut msg = " + result + "\n" + data);
                });
            NetManager.Instance.Request(NetProtocolEnum.QCustomsDataPut, cusData);
        }

    }
}

